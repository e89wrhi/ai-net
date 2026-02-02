using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Meeting.Features.StreamMeetingAnalysis.V1;

public class StreamMeetingAnalysisEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/meeting/analyze-stream",
                (StreamMeetingAnalysisRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    return mediator.CreateStream(new StreamMeetingAnalysisCommand(request.Transcript), cancellationToken);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StreamMeetingAnalysis")
            .WithApiVersionSet(builder.NewApiVersionSet("Meeting").Build())
            .Produces<IAsyncEnumerable<string>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Stream Meeting Analysis")
            .WithDescription("Streams the AI analysis of a meeting transcript.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
