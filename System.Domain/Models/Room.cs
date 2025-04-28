using System.Shared.BaseModel;

namespace System.Domain.Models
{
    public class Room : BaseEntity<int>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int StoreId { get; set; }
        public Store Store { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();
        public List<AssistanceRequest> AssistanceRequests { get; set; } = new List<AssistanceRequest>();
    }
}
