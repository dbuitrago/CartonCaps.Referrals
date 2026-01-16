using CartonCaps.Referrals.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CartonCaps.Referrals.Data
{
    public class ReferralDbContext(DbContextOptions<ReferralDbContext> options) : DbContext(options)
    {
        public DbSet<ReferralLinkEntity> ReferralLinks => Set<ReferralLinkEntity>();
        public DbSet<ReferralEntity> Referrals => Set<ReferralEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReferralLinkEntity>(e =>
            {
                e.HasKey(x => x.Id);

                e.Property(x => x.ReferrerCode).IsRequired();
                e.Property(x => x.IdempotencyKey).IsRequired();
                e.Property(x => x.VendorToken).IsRequired();
                e.Property(x => x.ReferralLinkUrl).IsRequired();
                e.HasIndex(x => new { x.ReferrerUserId, x.IdempotencyKey }).IsUnique();
                e.HasIndex(x => new { x.ReferrerUserId, x.CreatedUtc });
            });

            modelBuilder.Entity<ReferralEntity>(e =>
            {
                e.HasKey(x => x.Id);

                e.Property(x => x.ReferrerCode).IsRequired();
                e.Property(x => x.Status).IsRequired();
                e.HasIndex(x => new { x.ReferrerUserId, x.CreatedUtc });
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
