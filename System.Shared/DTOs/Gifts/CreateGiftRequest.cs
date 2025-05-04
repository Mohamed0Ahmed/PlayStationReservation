namespace System.Shared.DTOs.Gifts
{
    public class CreateGiftRequest
    {
        public string Name { get; set; }
        public int PointsRequired { get; set; }
        public int StoreId { get; set; }
    }
}