using AI.Common.Core;
using ChatBot.ValueObjects;

namespace ChatBot.Events;

public record MessageRespondedDomainEvent(SessionId SessionId, MessageId MessageId, string Response, int TokenUsed) : IDomainEvent;
