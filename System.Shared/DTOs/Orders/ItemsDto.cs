namespace System.Shared.DTOs.Orders
{
    public class ItemsDto
    {
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
