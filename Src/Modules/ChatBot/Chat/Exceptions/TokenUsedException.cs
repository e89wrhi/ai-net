using AI.Common.BaseExceptions;

namespace ChatBot.Exceptions;

public class TokenUsedException : DomainException
{
    public TokenUsedException(string token_used)
        : base($"token_used: '{token_used}' is invalid.")
    {
    }
}
