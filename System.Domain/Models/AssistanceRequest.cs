using System.Domain.Enums;
using System.Shared.BaseModel;

namespace System.Domain.Models
{
    public class Request : BaseEntity<int>
    {
        public int CustomerId { get; set; }
        public int RoomId { get; set; }
        public int StoreId { get; set; }
        public int RequestTypeId { get; set; }
        public Status Status { get; set; }
        public string RejectionReason { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
    }
}