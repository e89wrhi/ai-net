using AI.Common.Core;
using Ardalis.GuardClauses;
using ChatBot.Data;
using ChatBot.Dtos;
using ChatBot.Exceptions;
using ChatBot.ValueObjects;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace ChatBot.Features.GetChatById.V1;

internal class GetChatByIdHandler : IQueryHandler<GetChatByIdQuery, GetChatByIdResult>
{
    private readonly IMapper _mapper;
    private readonly ChatDbContext _dbContext;

    public GetChatByIdHandler(IMapper mapper, ChatDbContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public async Task<GetChatByIdResult> Handle(GetChatByIdQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var chat = await _dbContext.Chats
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == SessionId.Of(request.SessionId), cancellationToken);

        if (chat == null)
            throw new ChatNotFoundException(request.SessionId);

        // Verify ownership
        if (chat.UserId.Value != request.UserId)
            throw new UnauthorizedAccessException("You do not have access to this chat session.");

        var chatDto = _mapper.Map<ChatDto>(chat);

        return new GetChatByIdResult(chatDto);
    }
}

public record GetChatByIdResult(ChatDto Chat);

public record GetChatByIdResponseDto(ChatDto Chat);
