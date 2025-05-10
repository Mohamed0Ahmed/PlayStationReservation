using System.Shared.BaseModel;

namespace System.Domain.Models
{
    public class DefaultAssistanceRequestType : BaseEntity<int>
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }
    }
}