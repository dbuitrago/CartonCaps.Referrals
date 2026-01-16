namespace CartonCaps.Referrals.Domain.Dtos.V1
{
    public record CreateReferralLinkResponseDto(
        string ReferralLinkUrl,
        DateTime ExpiresUtc
    );
}
