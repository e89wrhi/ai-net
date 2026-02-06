using MediatR;

namespace SpeechToText.Features.StreamTranscribeAudio.V1;

public record StreamTranscribeAudioCommand(Guid UserId, string AudioUrl, string Language, string? ModelId = null) : IStreamRequest<string>;


