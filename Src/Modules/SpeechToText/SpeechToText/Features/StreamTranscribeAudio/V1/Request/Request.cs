using SpeechToText.Enums;

namespace SpeechToText.Features.StreamTranscribeAudio.V1;

public record StreamTranscribeAudioRequestDto(Guid UserId, string AudioUrl, string Language, bool IncludePunctuation, SpeechToTextDetailLevel DetailLevel, string? ModelId = null);

