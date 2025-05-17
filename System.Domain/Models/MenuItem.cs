using System.Shared.BaseModel;

namespace System.Domain.Models
{
    public class MenuItem : BaseEntity<int>
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        //public int PointsRequired { get; set; }
        public int CategoryId { get; set; }
        public int StoreId { get; set; }

    }
}