using CartonCaps.Referrals.Domain.Dtos.V1;

namespace CartonCaps.Referrals.Contracts.Services
{
    public interface IReferralService
    {
        Task<IReadOnlyList<ReferralDto>> ListAsync(Guid userId);
    }
}
