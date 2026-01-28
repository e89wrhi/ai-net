using AI.Common.Core;
using ChatBot.ValueObjects;

namespace ChatBot.Models;

public record MessageModel : Entity<MessageId>
{
    public SessionId SessionId { get; private set; } = default!;

    public MessageSender Sender { get; private set; } = default!;

    public MessageTime Time { get; private set; } = default!;

    public MessageContent Content { get; private set; } = default!;

    public TokenUsed TokenUsed { get; private set; } = default!;

    public MessageSender SenderType {  get; private set; } = default!;
    public bool IsEdited { get; private set; } = default!;
    public string Metadata { get; private set; } = default!;

    public static MessageModel Create(MessageId id, SessionId sessionId, MessageSender sender, MessageContent content, TokenUsed tokenUsed)
    {
        return new MessageModel
        {
            Id = id,
            SessionId = sessionId,
            Sender = sender,
            Content = content,
            TokenUsed = tokenUsed,
            Time = MessageTime.Of(DateTime.UtcNow),
            SenderType = sender,
            IsEdited = false,
            Metadata = string.Empty
        };
    }
}

