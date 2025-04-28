using System.Shared.BaseModel;

namespace System.Domain.Models
{
    public class PointSetting : BaseEntity<int>
    {
        public int StoreId { get; set; }
        public Store Store { get; set; }
        public decimal Amount { get; set; } 
        public int Points { get; set; }
    }
}