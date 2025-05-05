using System.Domain.Enums;
using System.Shared.BaseModel;

namespace System.Domain.Models
{
    public class Order : BaseEntity<int>
    {
        public int CustomerId { get; set; }
        public int RoomId { get; set; }
        public int StoreId { get; set; }
        public decimal TotalAmount { get; set; }
        public Status Status { get; set; }
        public string RejectionReason { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public List<OrderItem> OrderItems { get; set; } = [];
    }
}