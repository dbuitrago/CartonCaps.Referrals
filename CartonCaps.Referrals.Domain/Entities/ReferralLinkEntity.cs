using CartonCaps.Referrals.Domain.Enums;

namespace CartonCaps.Referrals.Domain.Entities
{
    public class ReferralLinkEntity
    {
        public Guid Id { get; set; }
        public Guid ReferrerUserId { get; set; }
        public string ReferrerCode { get; set; } = default!;
        public ChannelEnum Channel { get; set; }
        public string IdempotencyKey { get; set; } = default!;
        public string VendorToken { get; set; } = default!;
        public string ReferralLinkUrl { get; set; } = default!;
        public ReferralLinkStatusEnum Status { get; set; } = ReferralLinkStatusEnum.Active;
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresUtc { get; set; }
    }
}
