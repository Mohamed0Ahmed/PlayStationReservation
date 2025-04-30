using System.ComponentModel.DataAnnotations;

namespace MvcProject.Models
{
    public class CustomerLoginViewModel
    {
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        public string PhoneNumber { get; set; } = null!;
    }
}