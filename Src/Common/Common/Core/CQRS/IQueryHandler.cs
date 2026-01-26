using MediatR;

namespace AI.Common.Core;

/// <summary>
/// A handler specifically for IQuery. 
/// It's responsible for fetching the requested data and returning it to the caller.
/// </summary>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : notnull
{
}