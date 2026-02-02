using System.Runtime.CompilerServices;
using System.Text;
using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using CodeDebug.Data;
using CodeDebug.Enums;
using CodeDebug.Models;
using CodeDebug.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace CodeDebug.Features.StreamAnalyzeCode.V1;

public class StreamAnalyzeCodeEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/codedebug/analyze-stream",
                (StreamAnalyzeCodeRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    return mediator.CreateStream(new StreamAnalyzeCodeCommand(request.Code, request.Language), cancellationToken);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StreamAnalyzeCode")
            .WithApiVersionSet(builder.NewApiVersionSet("CodeDebug").Build())
            .Produces<IAsyncEnumerable<string>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Stream Analyze Code")
            .WithDescription("Streams the code analysis results using AI.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
