namespace CartonCaps.Referrals.Domain.Models
{
    public record VendorCreateLinkResultModel(string VendorToken, string Url, DateTime ExpiresUtc);
}
