using TextToSpeech.Enums;

namespace TextToSpeech.Features.SynthesizeSpeech.V1;

public record SynthesizeSpeechRequestDto(string Text, VoiceType Voice, SpeechSpeed Speed, string Language);
public record SynthesizeSpeechResponseDto(Guid SessionId, Guid ResultId, string AudioUrl);
