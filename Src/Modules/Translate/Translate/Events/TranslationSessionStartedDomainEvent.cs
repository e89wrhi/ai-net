using AI.Common.Core;
using AiOrchestration.ValueObjects;
using Translate.ValueObjects;

namespace Translate.Events;

public record TranslationSessionStartedDomainEvent(TranslateId Id, UserId UserId, ModelId AiModelId) : IDomainEvent;
