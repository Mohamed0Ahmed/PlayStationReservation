namespace System.Shared.DTOs
{
    public class UpdateMenuItemRequest
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int PointsRequired { get; set; }
    }
}