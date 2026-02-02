using System.Security.Claims;
using AI.Common.Caching;
using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using ChatBot.Data;
using ChatBot.Dtos;
using ChatBot.Exceptions;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace ChatBot.Features.GetChatHistory.V1;

public record GetChatHistory(Guid UserId) : IQuery<GetChatHistoryResult>, ICacheRequest
{
    public string CacheKey => $"GetChatHistory_{UserId}";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

public record GetChatHistoryResult(IEnumerable<ChatDto> ChatDtos);

public record GetChatHistoryResponseDto(IEnumerable<ChatDto> ChatDtos);

public class GetChatHistoryEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/chat/history",
                async (IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    
                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var result = await mediator.Send(new GetChatHistory(userId), cancellationToken);

                    var response = result.Adapt<GetChatHistoryResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetChatHistory")
            .WithApiVersionSet(builder.NewApiVersionSet("Chat").Build())
            .Produces<GetChatHistoryResponseDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Chat History")
            .WithDescription("Gets the chat session history for the currently authenticated user.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

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
