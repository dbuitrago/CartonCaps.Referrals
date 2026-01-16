using CartonCaps.Referrals.Domain.Entities;

namespace CartonCaps.Referrals.Contracts.Data.Repositories
{
    public interface IReferralRepository
    {
        Task<IReadOnlyList<ReferralEntity>> ListByReferrerAsync(Guid referrerUserId);
        Task<ReferralEntity> AddAsync(ReferralEntity entity);
    }
}
