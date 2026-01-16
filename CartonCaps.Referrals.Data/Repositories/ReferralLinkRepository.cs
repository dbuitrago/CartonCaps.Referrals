using CartonCaps.Referrals.Contracts.Data.Repositories;
using CartonCaps.Referrals.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CartonCaps.Referrals.Data.Repositories
{
    public class ReferralLinkRepository(ReferralDbContext db) : IReferralLinkRepository
    {
        private readonly ReferralDbContext _db = db;

        public Task<ReferralLinkEntity?> FindByVendorTokenAsync(string vendorToken) =>
            _db.ReferralLinks.AsNoTracking()
               .FirstOrDefaultAsync(x => x.VendorToken == vendorToken);

        public Task<ReferralLinkEntity?> FindByIdempotencyAsync(Guid userId, string idempotencyKey) =>
            _db.ReferralLinks.AsNoTracking()
               .FirstOrDefaultAsync(x => x.ReferrerUserId == userId && x.IdempotencyKey == idempotencyKey);

        public Task<int> CountCreatedSinceAsync(Guid userId, DateTime sinceUtc) =>
            _db.ReferralLinks.AsNoTracking()
               .CountAsync(x => x.ReferrerUserId == userId && x.CreatedUtc >= sinceUtc);

        public async Task<ReferralLinkEntity> AddAsync(ReferralLinkEntity entity)
        {
            await _db.ReferralLinks.AddAsync(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}
