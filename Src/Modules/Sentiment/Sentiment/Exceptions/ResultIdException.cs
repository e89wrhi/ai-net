using AI.Common.BaseExceptions;

namespace Sentiment.Exceptions;

public class ResultIdException : DomainException
{
    public ResultIdException(Guid id)
        : base($"result_id: '{id}' is invalid.")
    {
    }
}
