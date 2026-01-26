using MediatR;

namespace AI.Common.Core;

/// <summary>
/// The engine room for our commands. 
/// Every ICommand must have a corresponding ICommandHandler that contains the actual business logic.
/// </summary>
public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, Unit>
    where TCommand : ICommand<Unit>
{
}

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : notnull
{
}