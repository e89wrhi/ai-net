using AI.Common.Core;
using ChatBot.Enums;
using ChatBot.ValueObjects;

namespace ChatBot.Events;

public record ChatSessionFailedDomainEvent(SessionId Id, ChatFailureReason Reason): IDomainEvent;