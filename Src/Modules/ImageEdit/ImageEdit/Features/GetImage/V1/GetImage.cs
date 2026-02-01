using AI.Common.Caching;
using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using ImageEdit.Data;
using ImageEdit.Dtos;
using ImageEdit.Exceptions;
using Duende.IdentityServer.EntityFramework.Entities;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ImageEdit.Features.GetImageEditHistory.V1;

public record GetImage(Guid UserId) : IQuery<GetImageEditHistoryResult>, ICacheRequest
{
    public string CacheKey => $"GetImageEditHistory_{UserId}";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

public record GetImageEditHistoryResult(IEnumerable<ImageEditDto> ImageEditDtos);

public record GetImageEditHistoryResponseDto(IEnumerable<ImageEditDto> ImageEditDtos);

public class GetImageEditHistoryEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/imageedit/history/{{userId}}",
                async (Guid userId, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetImage(userId), cancellationToken);

                    var response = result.Adapt<GetImageEditHistoryResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetImageEditHistory")
            .WithApiVersionSet(builder.NewApiVersionSet("ImageEdit").Build())
            .Produces<GetImageEditHistoryResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get ImageEdit History")
            .WithDescription("Get ImageEdit History")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

internal class GetImageEditHistoryHandler : IQueryHandler<GetImage, GetImageEditHistoryResult>
{
    private readonly IMapper _mapper;
    private readonly ImageEditReadDbContext _readDbContext;

    public GetImageEditHistoryHandler(IMapper mapper, ImageEditReadDbContext readDbContext)
    {
        _mapper = mapper;
        _readDbContext = readDbContext;
    }

    public async Task<GetImageEditHistoryResult> Handle(GetImage request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var imageedits = await _readDbContext.ImageEdits.AsQueryable()
            .Where(x => x.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        var imageeditDtos = _mapper.Map<IEnumerable<ImageEditDto>>(imageedits);

        return new GetImageEditHistoryResult(imageeditDtos);
    }
}
