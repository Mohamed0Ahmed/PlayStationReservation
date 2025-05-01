using System.ComponentModel.DataAnnotations;
using System.Domain.Models;

namespace MvcProject.Models
{
    public class MenuItemViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Item name is required.")]
        public string Name { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative.")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Points required cannot be negative.")]
        public int PointsRequired { get; set; }

        public int MenuCategoryId { get; set; }
    }
}