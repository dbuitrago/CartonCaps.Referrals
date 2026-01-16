using CartonCaps.Referrals.Domain.Entities;

namespace CartonCaps.Referrals.Contracts.Data.Repositories
{
    public interface IReferralLinkResolveRepository
    {
        Task<ReferralLinkEntity?> FindByVendorTokenAsync(string vendorToken);
    }
}
