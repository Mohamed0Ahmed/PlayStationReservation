namespace System.Shared.DTOs.Requests
{
    public class RequestDto
    {
        public int Id { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public int? CustomerId { get; set; }
        public int RoomId { get; set; }
        public int StoreId { get; set; }
        public int RequestTypeId { get; set; }
        public Status Status { get; set; }
        public string RejectionReason { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
    }
}
