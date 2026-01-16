using CartonCaps.Referrals.Common.Models;
using CartonCaps.Referrals.Common.Services;
using CartonCaps.Referrals.Common.Vendor;
using CartonCaps.Referrals.Contracts.Data.Repositories;
using CartonCaps.Referrals.Contracts.Services;
using CartonCaps.Referrals.Domain.Dtos.V1;
using CartonCaps.Referrals.Domain.Entities;
using CartonCaps.Referrals.Domain.Enums;

namespace CartonCaps.Referrals.Business.Services
{
    public class ReferralLinkService : IReferralLinkService, IReferralResolveService
    {
        private readonly IReferralLinkRepository _referralLinkRepository;
        private readonly IReferralRepository _referralRepository;
        private readonly ILinkVendorClient _vendor;

        private const int MaxPerHour = 20;

        public ReferralLinkService(IReferralLinkRepository referralLinkRepository, IReferralRepository referralRepository, ILinkVendorClient vendor)
            => (_referralLinkRepository, _referralRepository, _vendor) = (referralLinkRepository, referralRepository, vendor);

        public async Task<CreateReferralLinkResponseDto> CreateAsync(Guid userId, string referrerCode, string channel, string idempotencyKey)
        {
            if (string.IsNullOrWhiteSpace(idempotencyKey))
                throw new MissingIdempotencyException("Missing Idempotency-Key header.");

            var existing = await _referralLinkRepository.FindByIdempotencyAsync(userId, idempotencyKey);
            if (existing is not null)
                return new CreateReferralLinkResponseDto(existing.ReferralLinkUrl, existing.ExpiresUtc);

            var since = DateTime.UtcNow.AddHours(-1);
            var createdLastHour = await _referralLinkRepository.CountCreatedSinceAsync(userId, since);
            if (createdLastHour >= MaxPerHour)
                throw new RateLimitExceededException($"Rate limit exceeded. Max {MaxPerHour}/hour.");

            if (!Enum.TryParse<ChannelEnum>(channel, ignoreCase: true, out var channelOut))
                throw new InvalidOperationException("Invalid channel.");

            var vendorResult = await _vendor.CreateReferralLinkAsync(referrerCode, channelOut);

            var entity = new ReferralLinkEntity
            {
                Id = Guid.NewGuid(),
                ReferrerUserId = userId,
                ReferrerCode = referrerCode,
                Channel = channelOut,
                IdempotencyKey = idempotencyKey.Trim(),
                VendorToken = vendorResult.VendorToken,
                ReferralLinkUrl = vendorResult.Url,
                ExpiresUtc = vendorResult.ExpiresUtc,
                Status = ReferralLinkStatusEnum.Active,
                CreatedUtc = DateTime.UtcNow
            };

            var saved = await _referralLinkRepository.AddAsync(entity);
            return new CreateReferralLinkResponseDto(saved.ReferralLinkUrl, saved.ExpiresUtc);
        }

        public async Task<ResolveReferralResponseDto> ResolveAsync(ResolveReferralRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.VendorToken))
                throw new InvalidOperationException("VendorToken is required.");

            if (string.IsNullOrWhiteSpace(request.FriendDisplayName))
                throw new InvalidOperationException("Friend DisplayName is required.");

            var link = await _referralLinkRepository.FindByVendorTokenAsync(request.VendorToken.Trim());

            if (link is null)
                return new ResolveReferralResponseDto(false, string.Empty, "Default");

            if (link.ExpiresUtc <= DateTime.UtcNow)
                throw new InvalidOperationException("Token expired.");

            await _referralRepository.AddAsync(new ReferralEntity
            {
                ReferrerCode = link.ReferrerCode,
                ReferrerUserId = link.ReferrerUserId,
                RedeemedUtc = DateTime.UtcNow,
                FriendDisplayName = request.FriendDisplayName
            });

            return new ResolveReferralResponseDto(true, link.ReferrerCode, "Referral");
        }
    }
}