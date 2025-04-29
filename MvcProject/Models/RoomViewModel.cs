using System.ComponentModel.DataAnnotations;

namespace MvcProject.Models
{
    public class RoomViewModel
    {
        public int Id { get; set; }

        public int StoreId { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, ErrorMessage = "Password cannot exceed 100 characters.")]
        public string Password { get; set; } = null!;
    }
}