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


public record GetMeetingSummary(Guid MeetingId) : IQuery<GetMeetingSummaryResult>, ICacheRequest
{
    public string CacheKey => $"GetMeetingSummary_{MeetingId}";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

public record GetMeetingSummaryResult(MeetingSummaryDto MeetingSummaryDto);

public record GetMeetingSummaryResponseDto(MeetingSummaryDto MeetingSummaryDto);

public class GetMeetingSummaryEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/meeting/{{meetingId}}/summary",
                async (Guid meetingId, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetMeetingSummary(meetingId), cancellationToken);

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

        var meeting = await _readDbContext.Meeting.AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == request.MeetingId, cancellationToken);

        if (meeting == null)
        {
            throw new MeetingNotFoundException(request.MeetingId);
        }

        var dto = _mapper.Map<MeetingSummaryDto>(meeting);

        return new GetMeetingSummaryResult(dto);
    }
}

