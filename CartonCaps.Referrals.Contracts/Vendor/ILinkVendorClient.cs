using CartonCaps.Referrals.Domain.Enums;
using CartonCaps.Referrals.Domain.Models;

namespace CartonCaps.Referrals.Common.Vendor
{
    public interface ILinkVendorClient
    {
        Task<VendorCreateLinkResultModel> CreateReferralLinkAsync(string referrerCode, ChannelEnum channel, string? token = default);
    }
}
