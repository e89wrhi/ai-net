
using System.Net;

namespace AI.Common.BaseExceptions;

/// <summary>
/// Thrown when a request conflicts with the current state of the server (e.g., trying to create a resource that already exists).
/// </summary>
public class ConflictException : CustomException
{
    public ConflictException(string message, int? code = null) : base(message, HttpStatusCode.Conflict, code: code)
    {
    }
}