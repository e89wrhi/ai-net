using AI.Common.BaseExceptions;

namespace AiOrchestration.Exceptions;

public class TokenCountException : DomainException
{
    public TokenCountException(long token_count)
        : base($"token_count: '{token_count}' is invalid.")
    {
    }
}
