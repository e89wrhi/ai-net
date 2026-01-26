using System.Net;

namespace AI.Common.BaseExceptions;

/// <summary>
/// Thrown when a business rule is violated within the Domain layer.
/// This prevents invalid state transitions in our aggregates.
/// </summary>
public class DomainException : CustomException
{
    public DomainException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest) : base(message, statusCode)
    {
    }

    public DomainException(string message, Exception innerException, HttpStatusCode statusCode = HttpStatusCode.BadRequest, int? code = null) : base(message, innerException, statusCode, code)
    {
    }
}