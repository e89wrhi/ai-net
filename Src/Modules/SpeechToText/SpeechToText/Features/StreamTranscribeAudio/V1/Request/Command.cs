using MediatR;

namespace SpeechToText.Features.StreamTranscribeAudio.V1;


public record StreamTranscribeAudioCommand(string AudioUrl, string Language) : IStreamRequest<string>;
