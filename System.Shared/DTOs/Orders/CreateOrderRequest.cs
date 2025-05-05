namespace System.Shared.DTOs.Orders
{
    public class CreateOrderRequest
    {
        public string PhoneNumber { get; set; }
        public int RoomId { get; set; }
        public List<ItemsDto> Items { get; set; }
    }
}
