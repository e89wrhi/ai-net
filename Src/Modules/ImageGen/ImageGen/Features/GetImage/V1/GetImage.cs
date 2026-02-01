using AI.Common.Caching;
using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using ImageGen.Data;
using ImageGen.Dtos;
using ImageGen.Exceptions;
using Duende.IdentityServer.EntityFramework.Entities;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ImageGen.Features.GetImageGenHistory.V1;

public record GetImage(Guid UserId) : IQuery<GetImageGenHistoryResult>, ICacheRequest
{
    public string CacheKey => $"GetImageGenHistory_{UserId}";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

public record GetImageGenHistoryResult(IEnumerable<ImageGenDto> ImageGenDtos);

public record GetImageGenHistoryResponseDto(IEnumerable<ImageGenDto> ImageGenDtos);

public class GetImageGenHistoryEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/imagegen/history/{{userId}}",
                async (Guid userId, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetImage(userId), cancellationToken);

                    var response = result.Adapt<GetImageGenHistoryResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetImageGenHistory")
            .WithApiVersionSet(builder.NewApiVersionSet("ImageGen").Build())
            .Produces<GetImageGenHistoryResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get ImageGen History")
            .WithDescription("Get ImageGen History")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

internal class GetImageGenHistoryHandler : IQueryHandler<GetImage, GetImageGenHistoryResult>
{
    private readonly IMapper _mapper;
    private readonly ImageGenReadDbContext _readDbContext;

    public GetImageGenHistoryHandler(IMapper mapper, ImageGenReadDbContext readDbContext)
    {
        _mapper = mapper;
        _readDbContext = readDbContext;
    }

    public async Task<GetImageGenHistoryResult> Handle(GetImage request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var imagegens = await _readDbContext.ImageGens.AsQueryable()
            .Where(x => x.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        var imagegenDtos = _mapper.Map<IEnumerable<ImageGenDto>>(imagegens);

        return new GetImageGenHistoryResult(imagegenDtos);
    }
}
