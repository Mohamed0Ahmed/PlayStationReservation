using System.Domain.Models;

namespace MvcProject.Models
{
    public class OwnerDashboardViewModel
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; } = null!;
        public List<MenuCategory> Categories { get; set; } = new();
        public List<Order> Orders { get; set; } = new();
        public List<AssistanceRequest> AssistanceRequests { get; set; } = new();
    }
}