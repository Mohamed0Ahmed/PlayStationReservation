

namespace System.Shared.DTOs.Orders
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public string CustomerNumber { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public int StoreId { get; set; }
        public decimal TotalAmount { get; set; }
        public Status Status { get; set; }
        public string RejectionReason { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public List<ItemsDto> OrderItems { get; set; } = [];
    }
}
