using TextToSpeech.Enums;

namespace TextToSpeech.Features.GenerateAudio.V1;

public record GenerateAudioRequestDto(string Text, VoiceType Voice, string? ModelId = null);
public record GenerateAudioResponseDto(Guid SessionId, string AudioUrl, string ModelId, string? ProviderName);
