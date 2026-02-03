using ChatBot.Data;
using ChatBot.Exceptions;
using ChatBot.ValueObjects;
using Ardalis.GuardClauses;
using AiOrchestration.Services;
using MediatR;

namespace ChatBot.Features.DeleteChat.V1;


internal class DeleteChatHandler : IRequestHandler<DeleteChatCommand, DeleteChatCommandResponse>
{
    private readonly ChatDbContext _dbContext;

    public DeleteChatHandler(ChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeleteChatCommandResponse> Handle(DeleteChatCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var chat = await _dbContext.Chats.FindAsync(new object[] { SessionId.Of(request.SessionId) }, cancellationToken);

        if (chat == null)
        {
            throw new ChatNotFoundException(request.SessionId);
        }

        chat.Delete();
        _dbContext.Chats.Remove(chat);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new DeleteChatCommandResponse(chat.Id.Value);
    }
}

