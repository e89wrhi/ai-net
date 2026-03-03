namespace AI.Common.Core;

/// <summary>
/// Defines the minimum properties every entity in our system must have.
/// This includes ID, Version (for concurrency), and Audit fields.
/// </summary>
public interface IEntity<T> : IEntity
{
    public T Id { get; set; }
}

public interface IEntity : IVersion
{
    public DateTime? CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public bool IsDeleted { get; set; }
}