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


    private readonly List<MessageModel> _messages = new();
    public IReadOnlyCollection<MessageModel> Messages => _messages.AsReadOnly(); 

}
