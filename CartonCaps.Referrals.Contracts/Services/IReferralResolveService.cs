using CartonCaps.Referrals.Domain.Dtos.V1;

namespace CartonCaps.Referrals.Contracts.Services
{
    public interface IReferralResolveService
    {
        Task<ResolveReferralResponseDto> ResolveAsync(ResolveReferralRequestDto request);
    }
}
