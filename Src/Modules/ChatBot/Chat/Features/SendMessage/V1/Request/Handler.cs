using ChatBot.Data;
using ChatBot.Exceptions;
using ChatBot.ValueObjects;
using MassTransit;
using MediatR;

namespace ChatBot.Features.SendMessage.V1;


internal class SendMessageHandler : IRequestHandler<SendMessageCommand, SendMessageCommandResponse>
{
    private readonly ChatDbContext _dbContext;

    public SendMessageHandler(ChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SendMessageCommandResponse> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var chat = await _dbContext.Chats.FindAsync(new object[] { SessionId.Of(request.SessionId) }, cancellationToken);

        if (chat == null)
        {
            throw new ChatNotFoundException(request.SessionId);
        }

        var message = ChatMessage.Create(
            MessageId.Of(NewId.NextGuid()),
            chat.Id,
            ValueObjects.MessageContent.Of(request.Sender),
            Models.MessageContent.Of(request.Content),
            ValueObjects.MaxTokens.Of(request.TokenUsed));

        chat.AddMessage(message);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SendMessageCommandResponse(message.Id.Value);
    }
}

