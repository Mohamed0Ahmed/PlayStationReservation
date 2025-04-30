using System.ComponentModel.DataAnnotations;
using System.Domain.Models;

namespace MvcProject.Models
{
    public class MenuItemViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Item name is required.")]
        [StringLength(100, ErrorMessage = "Item name cannot exceed 100 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10,000.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Points required is required.")]
        [Range(0, 10000, ErrorMessage = "Points required must be between 0 and 10,000.")]
        public int PointsRequired { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public int MenuCategoryId { get; set; }

        public List<MenuCategory> Categories { get; set; } = new();
    }
}