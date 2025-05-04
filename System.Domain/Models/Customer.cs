using System.Shared.BaseModel;

namespace System.Domain.Models
{
    public class Customer : BaseEntity<int>
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public int Points { get; set; }
        public int StoreId { get; set; }
        public Store Store { get; set; } = null!;
        public List<Order> Orders { get; set; } = new List<Order>();
        public List<AssistanceRequest> AssistanceRequests { get; set; } = new List<AssistanceRequest>();
        public List<GiftRedemption> GiftRedemptions { get; set; } = new List<GiftRedemption>();
    }
}