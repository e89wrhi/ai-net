using AI.Common.Core;
using TextToSpeech.Enums;

namespace TextToSpeech.Features.SynthesizeSpeech.V1;

public record SynthesizeSpeechCommand(string Text, VoiceType Voice, SpeechSpeed Speed, string Language, string? ModelId = null) : ICommand<SynthesizeSpeechCommandResult>;

public record SynthesizeSpeechCommandResult(Guid SessionId, Guid ResultId, string AudioUrl, string ModelId, string? ProviderName);

