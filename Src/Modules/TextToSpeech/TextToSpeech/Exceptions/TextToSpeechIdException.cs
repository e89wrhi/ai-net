using AI.Common.BaseExceptions;

namespace TextToSpeech.Exceptions;

public class TextToSpeechIdException : DomainException
{
    public TextToSpeechIdException(Guid id)
        : base($"texttospeech_id: '{id}' is invalid.")
    {
    }
}