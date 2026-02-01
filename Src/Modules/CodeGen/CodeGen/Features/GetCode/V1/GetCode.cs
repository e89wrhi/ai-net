using AI.Common.Caching;
using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using CodeGen.Data;
using CodeGen.Dtos;
using CodeGen.Exceptions;
using Duende.IdentityServer.EntityFramework.Entities;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace CodeGen.Features.GetCodeGenHistory.V1;

public record GetCode(Guid UserId) : IQuery<GetCodeGenHistoryResult>, ICacheRequest
{
    public string CacheKey => $"GetCodeGenHistory_{UserId}";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

public record GetCodeGenHistoryResult(IEnumerable<CodeGenDto> CodeGenDtos);

public record GetCodeGenHistoryResponseDto(IEnumerable<CodeGenDto> CodeGenDtos);

public class GetCodeGenHistoryEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/codegen/history/{{userId}}",
                async (Guid userId, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetCode(userId), cancellationToken);

                    var response = result.Adapt<GetCodeGenHistoryResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetCodeGenHistory")
            .WithApiVersionSet(builder.NewApiVersionSet("CodeGen").Build())
            .Produces<GetCodeGenHistoryResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get CodeGen History")
            .WithDescription("Get CodeGen History")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

internal class GetCodeGenHistoryHandler : IQueryHandler<GetCode, GetCodeGenHistoryResult>
{
    private readonly IMapper _mapper;
    private readonly CodeGenReadDbContext _readDbContext;

    public GetCodeGenHistoryHandler(IMapper mapper, CodeGenReadDbContext readDbContext)
    {
        _mapper = mapper;
        _readDbContext = readDbContext;
    }

    public async Task<GetCodeGenHistoryResult> Handle(GetCode request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var codegens = await _readDbContext.CodeGens.AsQueryable()
            .Where(x => x.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        var codegenDtos = _mapper.Map<IEnumerable<CodeGenDto>>(codegens);

        return new GetCodeGenHistoryResult(codegenDtos);
    }
}
