using MediatR;
using SpeechToText.Enums;

namespace SpeechToText.Features.StreamTranscribeAudio.V1;

public record StreamTranscribeAudioCommand(Guid UserId, string AudioUrl, string Language, bool IncludePunctuation, SpeechToTextDetailLevel DetailLevel, string? ModelId = null) : IStreamRequest<string>;


