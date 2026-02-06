namespace SpeechToText.Features.StreamTranscribeAudio.V1;

public record StreamTranscribeAudioRequestDto(string AudioUrl, string Language, string? ModelId = null);

