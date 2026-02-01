using AI.Common.BaseExceptions;

namespace LearningAssistant.Exceptions;

public class OutcomeException : DomainException
{
    public OutcomeException(string outcome)
        : base($"outcome: '{outcome}' is invalid.")
    {
    }
}
