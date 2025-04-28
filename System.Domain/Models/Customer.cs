using System.Shared.BaseModel;

namespace System.Domain.Models
{
    public class Customer : BaseEntity<int>
    {
        public string PhoneNumber { get; set; }
        public int Points { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();
        public List<AssistanceRequest> AssistanceRequests { get; set; } = new List<AssistanceRequest>();
    }
}
