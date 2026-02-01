using AI.Common.BaseExceptions;
using AI.Common.Core;
using AiOrchestration.ValueObjects;
using ChatBot.Enums;
using ChatBot.ValueObjects;

namespace ChatBot.Models;

public record ChatSession : Aggregate<SessionId>
{
    public UserId UserId { get; private set; } = default!;
    public SessionStatus SessionStatus { get; private set; }
    public string Title { get; private set; } = default!;
    public string Summary { get; private set; } = default!;
    public ModelId AiModelId { get; private set; } = default!;
    public ChatConfiguration Configuration { get; private set; } = default!;
    public TokenCount TotalTokens { get; private set; } = default!;
    public CostEstimate TotalCost { get; private set; } = default!;
    public DateTime LastSentAt { get; private set; }

    private readonly List<ChatMessage> _messages = new();
    public IReadOnlyCollection<ChatMessage> Messages => _messages.AsReadOnly();

    private ChatSession() { }

    public static ChatSession Create(
        SessionId id,
        UserId userId,
        string title,
        ModelId aiModelId,
        ChatConfiguration configuration)
    {
        var chat = new ChatSession
        {
            Id = id,
            UserId = userId,
            Title = title,
            AiModelId = aiModelId,
            Configuration = configuration,
            SessionStatus = SessionStatus.Active,
            CreatedAt = DateTime.UtcNow,
            LastSentAt = DateTime.UtcNow
        };

        chat.AddDomainEvent(
            new Events.ChatSessionStartedDomainEvent(id, userId, title, aiModelId));

        return chat;
    }

    public void AddMessage(ChatMessage message)
    {
        if (SessionStatus != SessionStatus.Active)
            throw new DomainException("Cannot add message to inactive session.");

        _messages.Add(message);
        LastSentAt = DateTime.UtcNow;
        var totalcount = TotalTokens.Value;
        TotalTokens = TokenCount.Of(totalcount += message.TokenUsed);
        var totalcost = TotalCost.Value;
        TotalCost = CostEstimate.Of(totalcost += message.Cost);

        AddDomainEvent(message.Sender switch
        {
            MessageSender.User =>
                new Events.MessageReceivedDomainEvent(
                    Id, message.Id, message.Content.Value, message.TokenUsed.Value),

            _ =>
                new Events.MessageRespondedDomainEvent(
                    Id, message.Id, message.Content.Value, message.TokenUsed.Value)
        });
    }

    public void Complete()
    {
        SessionStatus = SessionStatus.Completed;
        AddDomainEvent(new Events.ChatSessionCompletedDomainEvent(Id));
    }

    public void Fail(ChatFailureReason reason)
    {
        SessionStatus = SessionStatus.Failed;
        AddDomainEvent(new Events.ChatSessionFailedDomainEvent(Id, reason));
    }

    public void Delete()
    {
        SessionStatus = SessionStatus.Deleted;
        AddDomainEvent(new Events.ChatSessionDeletedDomainEvent(Id));
    }
}
