using Microsoft.Extensions.Logging;

namespace AI.Common.Polly;

using global::Polly;
using Exception = System.Exception;

/// <summary>
/// Infrastructure extensions for making our system more resilient using Polly.
/// It provides a simple way to retry operations (like database saves or API calls) if they fail due to transient errors.
/// </summary>
public static class Extensions
{
    public static ILogger Logger { get; set; } = null!;

    public static T RetryOnFailure<T>(this object retrySource, Func<T> action, int retryCount = 3)
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .Retry(retryCount, (exception, retryAttempt, context) =>
                               {
                                   Logger.LogInformation($"Retry attempt: {retryAttempt}");
                                   Logger.LogError($"Exception: {exception.Message}");
                               });

        return retryPolicy.Execute(action);
    }
}