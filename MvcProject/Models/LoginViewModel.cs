using System.ComponentModel.DataAnnotations;

namespace MvcProject.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } = null!;

        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}