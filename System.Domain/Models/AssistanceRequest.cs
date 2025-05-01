using System.Domain.Enums;
using System.Shared.BaseModel;

namespace System.Domain.Models
{
    public class AssistanceRequest : BaseEntity<int>
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; }
        public int RequestTypeId { get; set; }
        public AssistanceRequestType RequestType { get; set; }
        public AssistanceRequestStatus Status { get; set; }
        public string RejectionReason { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
    }
}