using CartonCaps.Referrals.Domain.Entities;

namespace CartonCaps.Referrals.Contracts.Data.Repositories
{
    public interface IReferralLinkRepository
    {
        Task<ReferralLinkEntity?> FindByVendorTokenAsync(string vendorToken);
        Task<ReferralLinkEntity?> FindByIdempotencyAsync(Guid userId, string idempotencyKey);
        Task<int> CountCreatedSinceAsync(Guid userId, DateTime sinceUtc);
        Task<ReferralLinkEntity> AddAsync(ReferralLinkEntity entity);
    }
}
