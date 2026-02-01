using AI.Common.BaseExceptions;

namespace AiOrchestration.Exceptions;

public class TokenCountException : DomainException
{
    public TokenCountException(int token_count)
        : base($"token_count: '{token_count}' is invalid.")
    {
    }
}
