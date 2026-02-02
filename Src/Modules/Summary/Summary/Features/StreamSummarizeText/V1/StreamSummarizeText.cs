using System.Runtime.CompilerServices;
using System.Text;
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

namespace Summary.Features.StreamSummarizeText.V1;

public class StreamSummarizeTextEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/summary/summarize-stream",
                (StreamSummarizeTextRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    return mediator.CreateStream(new StreamSummarizeTextCommand(request.Text, request.DetailLevel, request.Language), cancellationToken);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StreamSummarizeText")
            .WithApiVersionSet(builder.NewApiVersionSet("Summary").Build())
            .Produces<IAsyncEnumerable<string>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Stream Text Summary")
            .WithDescription("Streams the generated summary of the provided text.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
