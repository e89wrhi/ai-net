using AI.Common.Core;
using CodeDebug.Enums;
using CodeDebug.ValueObjects;

namespace CodeDebug.Events;

public record CodeDebugSessionFailedDomainEvent(CodeDebugId Id, CodeDebugFailureReason Reason): IDomainEvent;
