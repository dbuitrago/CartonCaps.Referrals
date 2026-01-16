using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartonCaps.Referrals.Domain.Entities
{
    public class ReferralEntity
    {
        public Guid Id { get; set; }
        public Guid ReferrerUserId { get; set; }
        public string ReferrerCode { get; set; } = default!;
        public string? FriendDisplayName { get; set; }
        public string Status { get; set; } = "Complete";
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public DateTime? RedeemedUtc { get; set; }
    }
}
