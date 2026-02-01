using AI.Common.BaseExceptions;

namespace SpeechToText.Exceptions;

public class SpeechToTextNotFoundException : DomainException
{
    public SpeechToTextNotFoundException(Guid speechtotextId)
        : base($"speechtotext: '{speechtotextId}' not found.")
    {
    }
}

public class SpeechToTextAlreadyExistException : DomainException
{
    public SpeechToTextAlreadyExistException(Guid speechtotextId)
        : base($"speechtotext: '{speechtotextId}' already exist.")
    {
    }
}
