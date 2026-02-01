using AI.Common.BaseExceptions;

namespace CodeGen.Exceptions;

public class ResultIdException : DomainException
{
    public ResultIdException(Guid result_id)
        : base($"result_id: '{result_id}' is invalid.")
    {
    }
}