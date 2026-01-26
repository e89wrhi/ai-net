
using System.Globalization;
using System.Net;

namespace AI.Common.BaseExceptions;

/// <summary>
/// A wrapper for unexpected errors that happen on the server side.
/// These should usually be logged and investigated.
/// </summary>
public class InternalServerException : CustomException
{
    public InternalServerException() : base() { }

    public InternalServerException(string message, int? code) : base(message, code: code) { }

    public InternalServerException(string message, int? code = null, params object[] args)
        : base(message: String.Format(CultureInfo.CurrentCulture, message, args, HttpStatusCode.InternalServerError, code))
    {
    }
}