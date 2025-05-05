using System.Shared.BaseModel;

namespace System.Domain.Models
{
    public class Category : BaseEntity<int>
    {
        public string Name { get; set; } = string.Empty;
        public int StoreId { get; set; }
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    }
}