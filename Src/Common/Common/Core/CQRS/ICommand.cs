using MediatR;

namespace AI.Common.Core;

/// <summary>
/// Represents a "Write" operation in our system.
/// Commands change state but usually don't return data (other than maybe a success flag or an ID).
/// </summary>
public interface ICommand : ICommand<Unit>
{
}

public interface ICommand<out T> : IRequest<T>
    where T : notnull
{
}