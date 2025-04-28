using System.Shared.BaseModel;

namespace System.Domain.Models
{
    public class Order : BaseEntity<int>
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } 
        public int PointsUsed { get; set; }
        public string Status { get; set; } 
        public string RejectionReason { get; set; }
        public DateTime OrderDate { get; set; }



        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}