using System.ComponentModel.DataAnnotations;

namespace MvcProject.Models
{
    public class MenuCategoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        public string Name { get; set; }
        public int StoreId { get; set; }
    }
}