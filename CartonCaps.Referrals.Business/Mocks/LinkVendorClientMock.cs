using CartonCaps.Referrals.Common.Vendor;
using CartonCaps.Referrals.Domain.Enums;
using CartonCaps.Referrals.Domain.Models;

namespace CartonCaps.Referrals.Business.Mocks
{
    public class LinkVendorClientMock : ILinkVendorClient
    {
        public Task<VendorCreateLinkResultModel> CreateReferralLinkAsync(string referrerCode, ChannelEnum channel, string? token = default)
        {
            var tokenGuid = string.IsNullOrEmpty(token) ? Guid.NewGuid().ToString("N") : token;
            var url = $"https://mock.vendor/link/{tokenGuid}?code={referrerCode}&ch={channel}";
            var expires = DateTime.UtcNow.AddDays(7);
            return Task.FromResult(new VendorCreateLinkResultModel(tokenGuid, url, expires));
        }
    }
}
