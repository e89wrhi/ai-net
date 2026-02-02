using AI.Common.Core;

namespace SpeechToText.Features.TranscribeAudio.V1;

public record TranscribeAudioCommand(string AudioUrl, string Language) : ICommand<TranscribeAudioCommandResult>;

public record TranscribeAudioCommandResult(Guid SessionId, Guid ResultId, string Transcript);
