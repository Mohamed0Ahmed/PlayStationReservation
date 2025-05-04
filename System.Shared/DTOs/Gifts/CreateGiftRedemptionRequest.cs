namespace System.Shared.DTOs.Gifts
{
    public class CreateGiftRedemptionRequest
    {
        public string PhoneNumber { get; set; }
        public int GiftId { get; set; }
        public int RoomId { get; set; }
    }
}