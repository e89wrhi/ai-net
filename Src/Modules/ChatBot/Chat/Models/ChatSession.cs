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

    private ChatSession() 
    { 
        _messages = new();
    }

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
            Summary = string.Empty,
            AiModelId = aiModelId,
            Configuration = configuration,
            SessionStatus = SessionStatus.Active,
            TotalTokens = TokenCount.Of(0),
            TotalCost = CostEstimate.Of(0),
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
        
        TotalTokens = TokenCount.Of((long)TotalTokens + (long)message.TokenUsed);
        TotalCost = CostEstimate.Of((decimal)TotalCost + (decimal)message.Cost);

        AddDomainEvent(message.Sender switch
        {
            MessageSender.User =>
                new Events.MessageReceivedDomainEvent(
                    Id, message.Id, message.Content.Value, (int)message.TokenUsed.Value),

            _ =>
                new Events.MessageRespondedDomainEvent(
                    Id, message.Id, message.Content.Value, (int)message.TokenUsed.Value)
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

    public void UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title cannot be empty.");

        Title = title;
        LastModified = DateTime.UtcNow;
    }

    public void Delete()
    {
        SessionStatus = SessionStatus.Deleted;
        AddDomainEvent(new Events.ChatSessionDeletedDomainEvent(Id));
    }
    public void UpdateSummary(string summary)
    {
        Summary = summary ?? string.Empty;
        LastModified = DateTime.UtcNow;
    }
}
