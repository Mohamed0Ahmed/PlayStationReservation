using System.ComponentModel.DataAnnotations;

namespace MvcProject.Models
{
    public class MenuCategoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public string Name { get; set; } = null!;
    }
}