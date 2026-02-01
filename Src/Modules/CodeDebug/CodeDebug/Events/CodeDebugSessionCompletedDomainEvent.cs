using AI.Common.Core;
using CodeDebug.ValueObjects;

namespace CodeDebug.Events;

public record CodeDebugSessionCompletedDomainEvent(CodeDebugId Id): IDomainEvent;
