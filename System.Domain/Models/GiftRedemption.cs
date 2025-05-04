using System.Shared.BaseModel;

namespace System.Domain.Models
{
    public class GiftRedemption : BaseEntity<int>
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        public int GiftId { get; set; }
        public Gift Gift { get; set; } = null!;
        public DateTime RedemptionDate { get; set; }
        public string Status { get; set; } = "Pending";
        public string RejectionReason { get; set; } = string.Empty;
    }
}