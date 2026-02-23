using AI.Common.Core;

namespace AI.Contracts.EventBus.Messages;

public record SpeechTranscribed(Guid Id, string Result, double Confidence) : IIntegrationEvent;
public record AudioProcessed(Guid Id, double DurationSeconds) : IIntegrationEvent;
public record SpeakerIdentified(Guid Id, string SpeakerId) : IIntegrationEvent;
public record TranscriptionCorrected(Guid Id) : IIntegrationEvent;
