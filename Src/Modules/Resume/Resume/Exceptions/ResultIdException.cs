using AI.Common.BaseExceptions;

namespace Resume.Exceptions;

public class ResultIdException : DomainException
{
    public ResultIdException(Guid id)
        : base($"result_id: '{id}' is invalid.")
    {
    }
}
