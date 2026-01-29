using AI.Common.Caching;
using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using ChatBot.Data;
using ChatBot.Dtos;
using ChatBot.Exceptions;
using Duende.IdentityServer.EntityFramework.Entities;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

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
        builder.MapGet($"{EndpointConfig.BaseApiPath}/chat/history/{{userId}}",
                async (Guid userId, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetChatHistory(userId), cancellationToken);

                    var response = result.Adapt<GetChatHistoryResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetChatHistory")
            .WithApiVersionSet(builder.NewApiVersionSet("Chat").Build())
            .Produces<GetChatHistoryResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Chat History")
            .WithDescription("Get Chat History")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

internal class GetChatHistoryHandler : IQueryHandler<GetChatHistory, GetChatHistoryResult>
{
    private readonly IMapper _mapper;
    private readonly ChatReadDbContext _readDbContext;

    public GetChatHistoryHandler(IMapper mapper, ChatReadDbContext readDbContext)
    {
        _mapper = mapper;
        _readDbContext = readDbContext;
    }

    public async Task<GetChatHistoryResult> Handle(GetChatHistory request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var chats = await _readDbContext.Chats.AsQueryable()
            .Where(x => x.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        var chatDtos = _mapper.Map<IEnumerable<ChatDto>>(chats);

        return new GetChatHistoryResult(chatDtos);
    }
}
