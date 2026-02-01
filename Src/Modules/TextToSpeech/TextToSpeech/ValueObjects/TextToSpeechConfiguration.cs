using TextToSpeech.Enums;
using TextToSpeech.Exceptions;

namespace TextToSpeech.ValueObjects;

public record TextToSpeechConfiguration
{
    public VoiceType Voice { get; }
    public SpeechSpeed Speed { get; }
    public LanguageCode Language { get; }

    public TextToSpeechConfiguration(VoiceType voice, SpeechSpeed speed, LanguageCode language)
    {
        Voice = voice;
        Speed = speed;
        Language = language;
    }
}
