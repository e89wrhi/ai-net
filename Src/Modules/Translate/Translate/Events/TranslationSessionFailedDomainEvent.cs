using AI.Common.Core;
using Translate.Enums;
using Translate.ValueObjects;

namespace Translate.Events;

public record TranslationSessionFailedDomainEvent(TranslateId Id, TranslationFailureReason Reason): IDomainEvent;
