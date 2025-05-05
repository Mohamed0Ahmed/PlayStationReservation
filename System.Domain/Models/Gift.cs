using System.Shared.BaseModel;

namespace System.Domain.Models
{
    public class Gift : BaseEntity<int>
    {
        public string Name { get; set; }
        public int PointsRequired { get; set; }
        public int StoreId { get; set; }
    }
}