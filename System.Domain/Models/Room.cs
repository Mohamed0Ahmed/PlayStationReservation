using System.Shared.BaseModel;

namespace System.Domain.Models
{
    public class Room : BaseEntity<int>
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int StoreId { get; set; }
        public List<Order> Orders { get; set; } = [];
        public List<Request> AssistanceRequests { get; set; } = [];
    }
}
