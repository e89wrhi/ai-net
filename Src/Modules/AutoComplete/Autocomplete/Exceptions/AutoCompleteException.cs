using AI.Common.BaseExceptions;

namespace AutoComplete.Exceptions;

public class AutoCompleteFoundException : DomainException
{
    public AutoCompleteFoundException(Guid id)
        : base($"autocomplete: '{id}' not found.")
    {
    }
}

public class AutoCompleteAlreadyExistException : DomainException
{
    public AutoCompleteAlreadyExistException(Guid id)
        : base($"autocomplete: '{id}' already exist.")
    {
    }
}
