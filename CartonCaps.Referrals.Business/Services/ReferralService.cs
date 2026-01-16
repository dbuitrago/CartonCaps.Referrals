using CartonCaps.Referrals.Contracts.Data.Repositories;
using CartonCaps.Referrals.Contracts.Services;
using CartonCaps.Referrals.Domain.Dtos.V1;

namespace CartonCaps.Referrals.Business.Services
{
    public class ReferralService(IReferralRepository referralRepository) : IReferralService
    {
        private readonly IReferralRepository _referralRepository = referralRepository;

        public async Task<IReadOnlyList<ReferralDto>> ListAsync(Guid userId)
        {
            var items = await _referralRepository.ListByReferrerAsync(userId);
            return items.Select(x => new ReferralDto(x.Id, x.FriendDisplayName, x.Status, x.CreatedUtc, x.RedeemedUtc)).ToList();
        }
    }
}
