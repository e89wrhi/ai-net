using AI.Common.BaseExceptions;

namespace AiOrchestration.Exceptions;

public class ProviderException : DomainException
{
    public ProviderException(string provider)
        : base($"provider: '{provider}' is invalid.")
    {
    }
}