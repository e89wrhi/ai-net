using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using Meeting.Data;
using Meeting.Models;
using Meeting.ValueObjects;
using Meeting.Enums;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace Meeting.Features.AnalyzeMeetingTranscript.V1;

public class AnalyzeMeetingTranscriptEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/meeting/analyze-transcript",
                async (AnalyzeMeetingTranscriptRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new AnalyzeMeetingTranscriptCommand(request.Transcript);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new AnalyzeMeetingTranscriptResponseDto(result.MeetingId, result.TranscriptId, result.Summary));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("AnalyzeMeetingTranscript")
            .WithApiVersionSet(builder.NewApiVersionSet("Meeting").Build())
            .Produces<AnalyzeMeetingTranscriptResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Analyze Meeting Transcript")
            .WithDescription("Uses AI to summarize a meeting transcript and extract key insights.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
