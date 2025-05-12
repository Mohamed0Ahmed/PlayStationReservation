namespace System.Shared.DTOs.Assistances
{
    public class CreateAssistanceRequest
    {
        public string CustomerNumber { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public int RequestTypeId { get; set; }
    }
}
