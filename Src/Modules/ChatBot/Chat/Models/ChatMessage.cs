using AI.Common.Core;
using AiOrchestration.ValueObjects;
using ChatBot.Enums;
using ChatBot.ValueObjects;

namespace ChatBot.Models;

public record ChatMessage : Entity<MessageId>
{
    public MessageSender Sender { get; private set; }
    public MessageContent Content { get; private set; } = default!;
    public TokenCount TokenUsed { get; private set; } = default!;
    public CostEstimate Cost { get; private set; } = default!;
    public DateTime SentAt { get; private set; }
    public int Sequence { get; private set; }

    private ChatMessage() { }

    public static ChatMessage Create(
        MessageId id,
        MessageSender sender,
        MessageContent content,
        TokenCount tokenUsed,
        CostEstimate cost,
        int sequence)
    {
        return new ChatMessage
        {
            Id = id,
            Sender = sender,
            Content = content,
            TokenUsed = tokenUsed,
            Cost = cost,
            Sequence = sequence,
            SentAt = DateTime.UtcNow
        };
    }
}
