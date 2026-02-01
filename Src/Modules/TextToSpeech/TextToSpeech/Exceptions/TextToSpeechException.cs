using AI.Common.BaseExceptions;

namespace TextToSpeech.Exceptions;

public class TextToSpeechNotFoundException : DomainException
{
    public TextToSpeechNotFoundException(Guid texttospeechId)
        : base($"texttospeech: '{texttospeechId}' not found.")
    {
    }
}

public class TextToSpeechAlreadyExistException : DomainException
{
    public TextToSpeechAlreadyExistException(Guid texttospeechId)
        : base($"texttospeech: '{texttospeechId}' already exist.")
    {
    }
}
