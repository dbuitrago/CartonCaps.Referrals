using CartonCaps.Referrals.Domain.Dtos.V1;

namespace CartonCaps.Referrals.Common.Services
{
    public interface IReferralLinkService
    {
        Task<CreateReferralLinkResponseDto> CreateAsync(Guid userId, string referrerCode, string channel, string idempotencyKey);
    }
}
