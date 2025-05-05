namespace System.Shared.DTOs.Menu
{
    public class ItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int PointsRequired { get; set; }
        public int CategoryId { get; set; }
        public int StoreId { get; set; }
    }
}
