using AI.Common.Core;
using Translate.ValueObjects;

namespace Translate.Events;

public record TextTranslatedDomainEvent(TranslateId Id, TranslateResultId ResultId, string Text): IDomainEvent;
