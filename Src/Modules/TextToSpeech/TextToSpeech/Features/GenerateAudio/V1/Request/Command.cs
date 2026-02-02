using AI.Common.Core;
using TextToSpeech.Enums;

namespace TextToSpeech.Features.GenerateAudio.V1;

public record GenerateAudioWithAICommand(string Text, VoiceType Voice) : ICommand<GenerateAudioWithAICommandResult>;

public record GenerateAudioWithAICommandResult(Guid SessionId, string AudioUrl);
