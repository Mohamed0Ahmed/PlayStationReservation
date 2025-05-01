using System.Shared.BaseModel;

namespace System.Domain.Models
{
    public class AssistanceRequestType : BaseEntity<int>
    {
        public string Name { get; set; }
        public int StoreId { get; set; }
        public Store Store { get; set; }
        public List<AssistanceRequest> AssistanceRequests { get; set; } = new List<AssistanceRequest>();
    }
}