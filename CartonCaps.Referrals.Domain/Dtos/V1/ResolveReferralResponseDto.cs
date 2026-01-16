namespace CartonCaps.Referrals.Domain.Dtos.V1
{
    public record ResolveReferralResponseDto(
        bool WasReferred,
        string ReferralCode,
        string GateVariant
    );
}
