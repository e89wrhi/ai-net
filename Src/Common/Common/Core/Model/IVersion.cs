namespace AI.Common.Core;

// For handling optimistic concurrency
/// <summary>
/// Used for optimistic concurrency tracking.
/// The version increments every time the record is saved, preventing users from overwriting each other's changes.
/// </summary>
public interface IVersion
{
    long Version { get; set; }
}