using AI.Common.BaseExceptions;

namespace User.Exceptions;

public class UserAnalyticsIdException : DomainException
{
    public UserAnalyticsIdException(Guid id)
        : base($"usage_analytics_id: '{id}' is invalid.")
    {
    }
}
