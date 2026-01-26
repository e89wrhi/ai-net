
namespace AI.Common.Caching;

/// <summary>
/// Use this interface for Commands that should clear a specific cache entry.
/// For example, after updating a Match, you'd want to invalidate the cached "GetMatchDetails" response.
/// </summary>
public interface IInvalidateCacheRequest
{
    string CacheKey { get; }
}