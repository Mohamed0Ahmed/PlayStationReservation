using System.Shared.BaseModel;

namespace System.Domain.Models
{
    public class OrderItem : BaseEntity<int>
    {
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal PriceAtOrderTime { get; set; }

    }
}
