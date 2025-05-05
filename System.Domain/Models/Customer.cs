using System.Shared.BaseModel;

namespace System.Domain.Models
{
    public class Customer : BaseEntity<int>
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public int Points { get; set; }
        public int StoreId { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();
        public List<Request> AssistanceRequests { get; set; } = new List<Request>();
    }
}