namespace SpeechToText.Features.TranscribeAudio.V1;

public record TranscribeAudioRequestDto(string AudioUrl, string Language, string? ModelId = null);
public record TranscribeAudioResponseDto(Guid SessionId, Guid ResultId, string Transcript, string ModelId, string? ProviderName);
