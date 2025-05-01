using System.ComponentModel.DataAnnotations;

namespace MvcProject.Models
{
    public class AssistanceRequestStatusUpdateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; }

        public string RejectionReason { get; set; }
    }
}