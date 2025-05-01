using System.Shared.BaseModel;

namespace System.Domain.Models
{
    public class MenuItem : BaseEntity<int>
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int PointsRequired { get; set; }
        public int MenuCategoryId { get; set; }
        public Category Category { get; set; }
    }
}