using MediatR;

namespace AI.Common.Core;

/// <summary>
/// Represents a "Read" operation.
/// Queries are used to fetch data from the system without modifying anything.
/// </summary>
public interface IQuery<out T> : IRequest<T>
    where T : notnull
{
}