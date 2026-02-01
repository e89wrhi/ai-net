using AI.Common.Caching;
using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using CodeDebug.Data;
using CodeDebug.Dtos;
using CodeDebug.Exceptions;
using Duende.IdentityServer.EntityFramework.Entities;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace CodeDebug.Features.GetCodeDebugHistory.V1;

public record GetCode(Guid UserId) : IQuery<GetCodeDebugHistoryResult>, ICacheRequest
{
    public string CacheKey => $"GetCodeDebugHistory_{UserId}";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

public record GetCodeDebugHistoryResult(IEnumerable<CodeDebugDto> CodeDebugDtos);

public record GetCodeDebugHistoryResponseDto(IEnumerable<CodeDebugDto> CodeDebugDtos);

public class GetCodeDebugHistoryEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/codedebug/history/{{userId}}",
                async (Guid userId, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetCode(userId), cancellationToken);

                    var response = result.Adapt<GetCodeDebugHistoryResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetCodeDebugHistory")
            .WithApiVersionSet(builder.NewApiVersionSet("CodeDebug").Build())
            .Produces<GetCodeDebugHistoryResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get CodeDebug History")
            .WithDescription("Get CodeDebug History")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

internal class GetCodeDebugHistoryHandler : IQueryHandler<GetCode, GetCodeDebugHistoryResult>
{
    private readonly IMapper _mapper;
    private readonly CodeDebugReadDbContext _readDbContext;

    public GetCodeDebugHistoryHandler(IMapper mapper, CodeDebugReadDbContext readDbContext)
    {
        _mapper = mapper;
        _readDbContext = readDbContext;
    }

    public async Task<GetCodeDebugHistoryResult> Handle(GetCode request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var codedebugs = await _readDbContext.CodeDebugs.AsQueryable()
            .Where(x => x.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        var codedebugDtos = _mapper.Map<IEnumerable<CodeDebugDto>>(codedebugs);

        return new GetCodeDebugHistoryResult(codedebugDtos);
    }
}
