using AI.Common.Core;
using SpeechToText.Enums;

namespace SpeechToText.Features.TranscribeAudio.V1;

public record TranscribeAudioCommand(Guid UserId, string AudioUrl, string Language, bool IncludePunctuation, SpeechToTextDetailLevel DetailLevel, string? ModelId = null) : ICommand<TranscribeAudioCommandResult>;

public record TranscribeAudioCommandResult(Guid SessionId, Guid ResultId, string Transcript, string ModelId, string? ProviderName);
