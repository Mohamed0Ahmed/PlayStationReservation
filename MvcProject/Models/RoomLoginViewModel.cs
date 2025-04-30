using System.ComponentModel.DataAnnotations;

namespace MvcProject.Models
{
    public class RoomLoginViewModel
    {
        [Required(ErrorMessage = "Store name is required.")]
        [StringLength(100, ErrorMessage = "Store name cannot exceed 100 characters.")]
        public string StoreName { get; set; } = null!;

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(50, ErrorMessage = "Password cannot exceed 50 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}