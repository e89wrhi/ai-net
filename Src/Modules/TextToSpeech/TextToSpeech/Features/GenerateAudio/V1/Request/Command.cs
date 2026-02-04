using AI.Common.Core;
using TextToSpeech.Enums;

namespace TextToSpeech.Features.GenerateAudio.V1;

public record GenerateAudioCommand(Guid UserId, string Text, VoiceType Voice, string? ModelId = null) : ICommand<GenerateAudioCommandResult>;

public record GenerateAudioCommandResult(Guid SessionId, string AudioUrl, string ModelId, string? ProviderName);
