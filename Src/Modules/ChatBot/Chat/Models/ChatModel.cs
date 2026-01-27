using AI.Common.Core;
using ChatBot.Enums;
using ChatBot.ValueObjects;

namespace ChatBot.Models;

public record ChatModel : Aggregate<SessionId>
{
    public UserId UserId { get; private set; } = default!;

    public SessionStatus SessionStatus { get; private set; } = default!;

    public DateTime LastSentAt { get; private set; } = default!;

    public string Title { get; private set; } = default!;
    public string Summary { get; private set; } = default!;
    public string AiModelId { get; private set; } = default!;
    public int TotalTokens { get; private set; } = default!;


    private readonly List<MessageModel> _messages = new();
    public IReadOnlyCollection<MessageModel> Messages => _messages.AsReadOnly(); 

    private ChatModel() { }

    public static ChatModel Create(SessionId id, UserId userId, string title, string aiModelId)
    {
        var chat = new ChatModel
        {
            Id = id,
            UserId = userId,
            Title = title,
            AiModelId = aiModelId,
            SessionStatus = SessionStatus.Active,
            CreatedAt = DateTime.UtcNow,
            LastSentAt = DateTime.UtcNow
        };

        chat.AddDomainEvent(new ChatBot.Events.ChatSessionStartedDomainEvent(id, userId, title));
        return chat;
    }

    public void AddMessage(MessageModel message)
    {
        _messages.Add(message);
        LastSentAt = DateTime.UtcNow;
        TotalTokens += message.TokenUsed.Value; // Assuming TokenUsed has a Value property or similar

        if (message.Sender == Enums.MessageSender.User.ToString())
        {
            AddDomainEvent(new ChatBot.Events.MessageRecievedDomainEvent(Id, message.Id, message.Content.Value));
        }
        else
        {
            AddDomainEvent(new ChatBot.Events.MessageRespondedDomainEvent(Id, message.Id, message.Content.Value));
        }
    }

    public void UpdateTitle(string title, string summary)
    {
        Title = title;
        Summary = summary;
        LastModified = DateTime.UtcNow;
    }
}
