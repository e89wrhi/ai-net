using AI.Common.BaseExceptions;

namespace AutoComplete.Exceptions;

public class AutoCompleteIdException : DomainException
{
    public AutoCompleteIdException(Guid id)
        : base($"autocomplete_id: '{id}' is invalid.")
    {
    }
}