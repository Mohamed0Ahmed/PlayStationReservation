using System.ComponentModel.DataAnnotations;

namespace MvcProject.Models
{
    public class AssistanceRequestViewModel
    {
        [Required(ErrorMessage = "Request type is required.")]
        [StringLength(50, ErrorMessage = "Request type cannot exceed 50 characters.")]
        public string RequestType { get; set; } = null!;
    }
}