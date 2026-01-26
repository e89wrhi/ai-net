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


}
