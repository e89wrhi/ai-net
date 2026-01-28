using AI.Common.Caching;
using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using Meeting.Data;
using Meeting.Dtos;
using Meeting.Exceptions;
using Duende.IdentityServer.EntityFramework.Entities;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Meeting.Features.GetMeetingSummary.V1;


public record GetMeetingSummary : IQuery<GetMeetingSummaryResult>, ICacheRequest
{
    public string CacheKey => "GetMeetingSummary";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

public record GetMeetingSummaryResult(IEnumerable<MeetingSummaryDto> MeetingDtos);

public record GetMeetingSummaryResponseDto(IEnumerable<MeetingSummaryDto> MeetingDtos);

public class GetMeetingSummaryEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetMeetingSummary(), cancellationToken);

                    var response = result.Adapt<GetMeetingSummaryResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetMeetingSummary")
            .WithApiVersionSet(builder.NewApiVersionSet("Meeting").Build())
            .Produces<GetMeetingSummaryResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Meeting Summary")
            .WithDescription("Get Meeting Summary")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

internal class GetMeetingSummaryHandler : IQueryHandler<GetMeetingSummary, GetMeetingSummaryResult>
{
    private readonly IMapper _mapper;
    private readonly MeetingReadDbContext _readDbContext;

    public GetMeetingSummaryHandler(IMapper mapper, MeetingReadDbContext readDbContext)
    {
        _mapper = mapper;
        _readDbContext = readDbContext;
    }

    public async Task<GetMeetingSummaryResult> Handle(GetMeetingSummary request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var result = (await _readDbContext.Meeting.AsQueryable().ToListAsync(cancellationToken))
            .Where(i => i.Id == request.Id);

        if (!result.Any())
        {
            throw new MeetingNotFoundException(request.Id);
        }

        var eventDtos = _mapper.Map<IEnumerable<MeetingSummaryDto>>(result);

        return new GetMeetingSummaryResult(eventDtos);
    }
}
