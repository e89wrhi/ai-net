using AI.Common.BaseExceptions;

namespace SpeechToText.Exceptions;

public class SpeechToTextIdException : DomainException
{
    public SpeechToTextIdException(Guid id)
        : base($"speechtotext_id: '{id}' is invalid.")
    {
    }
}