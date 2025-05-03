namespace System.Shared.DTOs
{
    public class CreateOrderRequest
    {
        public string PhoneNumber { get; set; }
        public int RoomId { get; set; }
        public List<(int menuItemId, int quantity)> Items { get; set; }
    }
}
