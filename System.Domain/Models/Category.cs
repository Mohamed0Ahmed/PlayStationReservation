using System.Shared.BaseModel;

namespace System.Domain.Models
{
    public class Category : BaseEntity<int>
    {
        public string Name { get; set; }
        public int StoreId { get; set; }
        public Store Store { get; set; }
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    }
}