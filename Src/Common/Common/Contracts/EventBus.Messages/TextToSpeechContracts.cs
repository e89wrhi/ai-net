using AI.Common.Core;

namespace AI.Contracts.EventBus.Messages;

public record SpeechSynthesized(Guid Id, string VoiceName, double DurationSeconds) : IIntegrationEvent;
public record AudioFormatConverted(Guid Id, string Format) : IIntegrationEvent;
public record VoiceProfileUpdated(Guid Id, string VoiceId) : IIntegrationEvent;
public record SynthesisFailed(Guid Id, string Error) : IIntegrationEvent;
