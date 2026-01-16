namespace CartonCaps.Referrals.Domain.Dtos.V1
{
    public record ReferralDto(
        Guid Id,
        string? FriendDisplayName,
        string Status,
        DateTime CreatedUtc,
        DateTime? RedeemedUtc
    );
}
