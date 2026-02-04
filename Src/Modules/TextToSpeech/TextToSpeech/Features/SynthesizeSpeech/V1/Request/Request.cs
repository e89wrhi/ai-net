using TextToSpeech.Enums;

namespace TextToSpeech.Features.SynthesizeSpeech.V1;

public record SynthesizeSpeechRequestDto(string Text, VoiceType Voice, SpeechSpeed Speed, string Language, string? ModelId = null);
public record SynthesizeSpeechResponseDto(Guid SessionId, Guid ResultId, string AudioUrl, string ModelId, string? ProviderName);
