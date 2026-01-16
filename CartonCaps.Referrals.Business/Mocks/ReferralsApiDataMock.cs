using CartonCaps.Referrals.Common.Vendor;
using CartonCaps.Referrals.Domain.Entities;
using CartonCaps.Referrals.Domain.Enums;

namespace CartonCaps.Referrals.Business.Mocks
{
    public static class ReferralsApiDataMock
    {
        //This class is only for mock and testing purpose, I decided ignore this warning
        #pragma warning disable CA2211
        public static string VendorToken = "4bc43cd5c2394faa952ef96b0ee047a5";
        public static string IdempotencyKey = "3e33e682fe014c0090cf0d7305994a76";
        public static string ReferrerCode = "XY7G4D";
        public static Guid UserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        #pragma warning restore CA2211

        public static async Task<ReferralLinkEntity> GetReferalLinkInitialData(ILinkVendorClient linkVendorClient)
        {
            var link = await linkVendorClient.CreateReferralLinkAsync(ReferrerCode, ChannelEnum.ShareSheet, VendorToken);
            return new ReferralLinkEntity
            {
                Id = Guid.NewGuid(),
                ReferrerUserId = UserId,
                ReferrerCode = ReferrerCode,
                Channel = ChannelEnum.ShareSheet,
                CreatedUtc = DateTime.UtcNow,
                VendorToken = VendorToken,
                IdempotencyKey = IdempotencyKey,
                Status = ReferralLinkStatusEnum.Active,
                ExpiresUtc = link.ExpiresUtc,
                ReferralLinkUrl = link.Url
            };
        }

        public static IEnumerable<ReferralEntity> GetReferalsInitialData()
        {
            return new List<ReferralEntity>(){
                new() {
                    Id = Guid.NewGuid(),
                    ReferrerUserId = UserId,
                    ReferrerCode = ReferrerCode,
                    FriendDisplayName = "Cindy Holguin",
                    Status = "Complete",
                    CreatedUtc = DateTime.UtcNow.AddDays(-3),
                    RedeemedUtc = DateTime.UtcNow.AddDays(-2)
                },
                new() {
                    Id = Guid.NewGuid(),
                    ReferrerUserId = UserId,
                    ReferrerCode = ReferrerCode,
                    FriendDisplayName = "Natalia Buitrago",
                    Status = "Pending",
                    CreatedUtc = DateTime.UtcNow.AddDays(-1)
                } 
            };
        }
    }
}
