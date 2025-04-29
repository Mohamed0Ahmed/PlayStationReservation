using System.ComponentModel.DataAnnotations;
using System.Domain.Models;

namespace MvcProject.Models
{
    public class StoreViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Store name is required.")]
        [StringLength(100, ErrorMessage = "Store name cannot exceed 100 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Owner email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string OwnerEmail { get; set; } = null!;

        public List<Room>? Rooms { get; set; }
    }
}