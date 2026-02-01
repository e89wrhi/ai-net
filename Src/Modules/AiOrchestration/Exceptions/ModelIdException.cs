using AI.Common.BaseExceptions;

namespace AiOrchestration.Exceptions;

public class ModelIdException : DomainException
{
    public ModelIdException(Guid modelId)
        : base($"model_id: '{modelId}' is invalid.")
    {
    }
}