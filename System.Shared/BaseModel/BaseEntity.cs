namespace System.Shared.BaseModel
{
    public abstract class BaseEntity<TKey> : IEntity where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedOn { get; set; }
        public bool IsHidden { get; set; } = false;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime LastModifiedOn { get; set; } = DateTime.UtcNow;
        public string? LastModifiedBy { get; set; }
    }
}