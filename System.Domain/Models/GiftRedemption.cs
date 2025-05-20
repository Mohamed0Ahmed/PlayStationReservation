using System.Domain.Enums;
using System.Shared.BaseModel;

namespace System.Domain.Models
{
    public class GiftRedemption : BaseEntity<int>
    {
        public int GiftId { get; set; }
        public string GiftName { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string CustomerNumber  { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public int StoreId { get; set; }
        public int PointsUsed { get; set; }
        public Status Status { get; set; }
        public string RejectionReason { get; set; } = string.Empty;
        public bool? IsCustomerConfirmed { get; set; }
    }
}
