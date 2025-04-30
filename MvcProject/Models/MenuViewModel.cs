using System.Domain.Models;

namespace MvcProject.Models
{
    public class MenuViewModel
    {
        public int CustomerPoints { get; set; }
        public List<MenuCategory> Categories { get; set; } = new();
    }
}