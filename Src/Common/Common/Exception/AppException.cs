using System.Net;

namespace AI.Common.BaseExceptions;

/// <summary>
/// A generic exception for application-level errors.
/// Defaults to a BadRequest status unless specified otherwise.
/// </summary>
public class AppException : CustomException
{
    public AppException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest, int? code = null) : base(message, statusCode, code: code)
    {
    }

    public AppException(string message, System.Exception innerException, HttpStatusCode statusCode = HttpStatusCode.BadRequest, int? code = null) : base(message, innerException, statusCode, code)
    {
    }
}