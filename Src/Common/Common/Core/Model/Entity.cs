namespace AI.Common.Core;

/// <summary>
/// The base class for all objects that have a unique identity in our system.
/// It also includes common "Audit" fields to track who created/modified the record and when.
/// </summary>
public abstract record Entity<T> : IEntity<T>
{
    public T Id { get; set; }
    public DateTime? CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public bool IsDeleted { get; set; }
    public long Version { get; set; }
}