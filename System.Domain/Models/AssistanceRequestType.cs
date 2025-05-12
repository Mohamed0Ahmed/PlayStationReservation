using System.Shared.BaseModel;

namespace System.Domain.Models
{
    public class AssistanceRequestType : BaseEntity<int>
    {
        public string Name { get; set; }
        public int StoreId { get; set; }
        //public string ImagePath { get; set; }

    }
}