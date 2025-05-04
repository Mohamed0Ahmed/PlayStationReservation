using System.Shared.BaseModel;

namespace System.Domain.Models
{
    public class OrderItem : BaseEntity<int>
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
        public int MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; } = null!;
        public int Quantity { get; set; } = 1;
    }
}