using AI.Common.BaseExceptions;

namespace AiOrchestration.Exceptions;

public class CostEstimateException : DomainException
{
    public CostEstimateException(decimal cost)
        : base($"cost_estimate: '{cost}' not found.")
    {
    }
}
