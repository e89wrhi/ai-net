
using System.Net;

namespace AI.Common.BaseExceptions;

/// <summary>
/// Thrown when a requested resource (like a specific Match or User) does not exist in the system.
/// </summary>
public class NotFoundException : CustomException
{
    public NotFoundException(string message, int? code = null) : base(message, HttpStatusCode.NotFound, code: code)
    {
    }
}