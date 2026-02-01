using AI.Common.Core;
using ChatBot.ValueObjects;

namespace ChatBot.Events;

public record MessageRespondedDomainEvent(SessionId Id, MessageId MessageId, string Content, int TokenUsed) : IDomainEvent;
