namespace System.Shared.BaseModel
{
    public interface IEntity
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedOn { get; set; }
        bool IsHidden { get; set; }
        DateTime CreatedOn { get; set; }
        string? CreatedBy { get; set; }
        DateTime LastModifiedOn { get; set; }
        string? LastModifiedBy { get; set; }
    }
}