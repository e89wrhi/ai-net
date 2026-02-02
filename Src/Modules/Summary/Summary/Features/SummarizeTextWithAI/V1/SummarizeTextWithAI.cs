using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using Summary.Data;
using Summary.Models;
using Summary.ValueObjects;
using Summary.Enums;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace Summary.Features.SummarizeTextWithAI.V1;

public class SummarizeTextWithAIEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/summary/summarize",
                async (SummarizeTextWithAIRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new SummarizeTextWithAICommand(request.Text, request.DetailLevel, request.Language);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new SummarizeTextWithAIResponseDto(result.SessionId, result.ResultId, result.Summary));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("SummarizeTextWithAI")
            .WithApiVersionSet(builder.NewApiVersionSet("Summary").Build())
            .Produces<SummarizeTextWithAIResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Summarize Text")
            .WithDescription("Uses AI to generate a summary of the provided text based on the specified detail level.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
