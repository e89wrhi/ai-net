namespace AI.Common.Caching;

/// <summary>
/// Mark your MediatR requests with this interface if you want them to be cached.
/// You'll need to provide a unique CacheKey and optionally an expiration time.
/// </summary>
public interface ICacheRequest
{
    string CacheKey { get; }
    DateTime? AbsoluteExpirationRelativeToNow { get; }
}