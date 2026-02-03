using AI.Common.Core;
using Ardalis.GuardClauses;
using ChatBot.Data;
using ChatBot.Dtos;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace ChatBot.Features.GetChatHistory.V1;

internal class GetChatHistoryHandler : IQueryHandler<GetChatHistory, GetChatHistoryResult>
{
    private readonly IMapper _mapper;
    private readonly ChatDbContext _dbContext;

    public GetChatHistoryHandler(IMapper mapper, ChatDbContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public async Task<GetChatHistoryResult> Handle(GetChatHistory request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var chats = await _dbContext.Chats
            .Where(x => x.UserId.Value == request.UserId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        var chatDtos = _mapper.Map<IEnumerable<ChatDto>>(chats);

        return new GetChatHistoryResult(chatDtos);
    }
}

