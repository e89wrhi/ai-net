using EasyCaching.Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AI.Common.Caching;

/// <summary>
/// A MediatR pipeline behavior that clears cached data whenever a relevant command is executed.
/// It looks for the IInvalidateCacheRequest interface to know which cache key needs to be purged.
/// </summary>
public class InvalidateCachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
        where TResponse : notnull
{
    private readonly ILogger<InvalidateCachingBehavior<TRequest, TResponse>> _logger;
    private readonly IEasyCachingProvider _cachingProvider;

    public InvalidateCachingBehavior(IEasyCachingProviderFactory cachingFactory,
        ILogger<InvalidateCachingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
        _cachingProvider = cachingFactory.GetCachingProvider("mem");
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not IInvalidateCacheRequest invalidateCacheRequest)
        {
            // No cache request found, so just continue through the pipeline
            return await next();
        }

        var cacheKey = invalidateCacheRequest.CacheKey;
        var response = await next();

        await _cachingProvider.RemoveAsync(cacheKey);

        _logger.LogDebug("Cache data with cache key: {CacheKey} removed.", cacheKey);

        return response;
    }
}