
using System.Net;

namespace AI.Common.BaseExceptions;

/// <summary>
/// Thrown when input validation fails (e.g., missing required fields or invalid formats).
/// </summary>
public class ValidationException : CustomException
{
    public ValidationException(string message, int? code = null) : base(message, HttpStatusCode.BadRequest, code: code)
    {
    }
}