using CartonCaps.Referrals.Contracts.Data.Repositories;
using CartonCaps.Referrals.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CartonCaps.Referrals.Data.Repositories
{
    public class ReferralRepository(ReferralDbContext db) : IReferralRepository
    {
        private readonly ReferralDbContext _db = db;

        public async Task<IReadOnlyList<ReferralEntity>> ListByReferrerAsync(Guid referrerUserId) =>
            await _db.Referrals.AsNoTracking()
                .Where(x => x.ReferrerUserId == referrerUserId)
                .OrderByDescending(x => x.CreatedUtc)
                .ToListAsync();

        public async Task<ReferralEntity> AddAsync(ReferralEntity entity)
        {
            await _db.Referrals.AddAsync(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}
