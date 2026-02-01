using AI.Common.BaseExceptions;

namespace AiOrchestration.Exceptions;

public class LatencyBudgetException : DomainException
{
    public LatencyBudgetException(decimal budget)
        : base($"latency_budget: '{budget}' is invalid.")
    {
    }
}