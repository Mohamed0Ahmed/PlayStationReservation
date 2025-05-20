
namespace System.Shared.DTOs.GiftRedemptions
{
    public class GiftRedemptionDto
    {
        public int Id { get; set; }
        public int GiftId { get; set; }
        public string GiftName { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string CustomerPhone { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public int StoreId { get; set; }
        public int PointsUsed { get; set; }
        public string Status { get; set; } = string.Empty;
        public string RejectionReason { get; set; } = string.Empty;
        public bool? IsCustomerConfirmed { get; set; }
    }

    public class CreateGiftRedemptionDto
    {
        public int GiftId { get; set; }
        public string CustomerNumber { get; set; } = string.Empty;
        public int RoomId { get; set; }
    }

    public class UpdateGiftRedemptionStatusDto
    {
        public bool IsApproved { get; set; }
        public string? RejectionReason { get; set; }
    }

}
