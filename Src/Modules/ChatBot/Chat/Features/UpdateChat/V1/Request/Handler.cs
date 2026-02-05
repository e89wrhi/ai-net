using AI.Common.Core;
using Ardalis.GuardClauses;
using ChatBot.Data;
using ChatBot.Exceptions;
using ChatBot.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace ChatBot.Features.UpdateChat.V1;

internal class UpdateChatHandler : ICommandHandler<UpdateChatCommand, UpdateChatCommandResponse>
{
    private readonly ChatDbContext _dbContext;

    public UpdateChatHandler(ChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UpdateChatCommandResponse> Handle(UpdateChatCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.NullOrWhiteSpace(request.Title, nameof(request.Title));

        var chat = await _dbContext.Chats
            .FirstOrDefaultAsync(c => c.Id == SessionId.Of(request.SessionId), cancellationToken);

        if (chat == null)
            throw new ChatNotFoundException(request.SessionId);

        // Verify ownership
        if (chat.UserId.Value != request.UserId)
            throw new UnauthorizedAccessException("You do not have access to this chat session.");

        // Update the title
        chat.UpdateTitle(request.Title);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateChatCommandResponse(true);
    }
}
