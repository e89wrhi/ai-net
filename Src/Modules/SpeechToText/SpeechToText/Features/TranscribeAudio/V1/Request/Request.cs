namespace SpeechToText.Features.TranscribeAudio.V1;

public record TranscribeAudioRequestDto(string AudioUrl, string Language);
public record TranscribeAudioResponseDto(Guid SessionId, Guid ResultId, string Transcript);
