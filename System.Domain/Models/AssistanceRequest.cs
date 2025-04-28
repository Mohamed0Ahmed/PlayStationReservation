using System.Shared.BaseModel;

namespace System.Domain.Models
{
    public class AssistanceRequest : BaseEntity<int>
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; }
        public string RequestType { get; set; } 
        public string Status { get; set; } 
        public string RejectionReason { get; set; }
        public DateTime RequestDate { get; set; }
    }
}