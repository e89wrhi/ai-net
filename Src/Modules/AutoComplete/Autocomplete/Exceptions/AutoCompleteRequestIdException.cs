using AI.Common.BaseExceptions;

namespace AutoComplete.Exceptions;

public class AutoCompleteRequestIdException : DomainException
{
    public AutoCompleteRequestIdException(Guid requestId)
        : base($"request_id: '{requestId}' is invalid.")
    {
    }
}