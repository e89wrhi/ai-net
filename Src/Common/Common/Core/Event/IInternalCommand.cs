namespace AI.Common.Core;

/// <summary>
/// A command that is triggered by a domain event but should be executed asynchronously within the same module's context.
/// </summary>
public interface IInternalCommand : IEvent
{
}