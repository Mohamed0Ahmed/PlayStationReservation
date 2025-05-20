using System.ComponentModel.DataAnnotations;
using System.Shared.BaseModel;

namespace System.Domain.Models
{
    public class Customer : BaseEntity<int>
    {
        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [RegularExpression(@"^(010|011|012|015)[0-9]{8}$", ErrorMessage = "من فضلك أدخل رقم هاتف مصري صحيح")]
        public string PhoneNumber { get; set; } = string.Empty;

        public int Points { get; set; }
        public int StoreId { get; set; }
        public List<Order> Orders { get; set; } = [];
        public List<Request> AssistanceRequests { get; set; } = [];
        //public List<GiftRedemption> GiftRedemptions { get; set; } = [];
    }
}