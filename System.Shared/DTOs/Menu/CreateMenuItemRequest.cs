namespace System.Shared.DTOs.Menu
{
    public class CreateMenuItemRequest
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int PointsRequired { get; set; }
        public int CategoryId { get; set; }
    }
}
