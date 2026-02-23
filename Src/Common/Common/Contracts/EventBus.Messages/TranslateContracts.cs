using AI.Common.Core;

namespace AI.Contracts.EventBus.Messages;

public record TextTranslated(Guid Id, string SourceLang, string TargetLang) : IIntegrationEvent;
public record LanguageDetected(Guid Id, string Language, double Confidence) : IIntegrationEvent;
public record TranslationModelUpdated(Guid Id, string ModelVersion) : IIntegrationEvent;
public record GlossaryApplied(Guid Id, string GlossaryId) : IIntegrationEvent;
