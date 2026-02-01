using AI.Common.BaseExceptions;

namespace ImageEdit.Exceptions;

public class ResultIdException : DomainException
{
    public ResultIdException(Guid id)
        : base($"result_id: '{id}' is invalid.")
    {
    }
}