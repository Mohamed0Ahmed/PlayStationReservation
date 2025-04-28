using System.Shared.BaseModel;

namespace System.Domain.Models
{
    public class Store : BaseEntity<int>
    {
        public string Name { get; set; }
        public string OwnerEmail { get; set; }
        public List<Room> Rooms { get; set; } = new List<Room>();
        public List<MenuCategory> MenuCategories { get; set; } = new List<MenuCategory>();
        public List<PointSetting> PointSettings { get; set; } = new List<PointSetting>();
    }
}
