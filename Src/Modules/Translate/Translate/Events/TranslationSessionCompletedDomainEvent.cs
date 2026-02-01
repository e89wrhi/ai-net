using AI.Common.Core;
using Translate.ValueObjects;

namespace Translate.Events;

public record TranslationSessionCompletedDomainEvent(TranslateId Id): IDomainEvent;
