using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace SimpleMD.Features.StreamSummarizeMD.V1;

public class StreamSummarizeMDEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/md/stream_summarize",
                async (StreamSummarizeMDRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                        return Results.Unauthorized();

                    var httpContext = httpContextAccessor.HttpContext!;
                    var command = new StreamSummarizeMDCommand(userId, request.Instruction, request.ModelId);
                    var result = await mediator.Send(command, cancellationToken);

                    // Write Server-Sent Events directly to the response.
                    httpContext.Response.ContentType = "text/event-stream";
                    httpContext.Response.Headers.CacheControl = "no-cache";
                    httpContext.Response.Headers.Connection = "keep-alive";

                    // First event: model metadata as JSON.
                    var meta = System.Text.Json.JsonSerializer.Serialize(
                        new StreamSummarizeMDResponseDto(result.ModelId, result.ProviderName));
                    await httpContext.Response.WriteAsync($"event: meta\ndata: {meta}\n\n", cancellationToken);

                    // Subsequent events: streamed summary chunks.
                    await foreach (var chunk in result.TextStream.WithCancellation(cancellationToken))
                    {
                        var escaped = chunk.Replace("\n", "\\n");
                        await httpContext.Response.WriteAsync($"data: {escaped}\n\n", cancellationToken);
                        await httpContext.Response.Body.FlushAsync(cancellationToken);
                    }

                    // Signal end of stream.
                    await httpContext.Response.WriteAsync("event: done\ndata: {}\n\n", cancellationToken);
                    return Results.Empty;
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StreamSummarizeMD")
            .WithApiVersionSet(builder.NewApiVersionSet("SimpleMD").Build())
            .Produces(StatusCodes.Status200OK, contentType: "text/event-stream")
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Stream Summarize MD File")
            .WithDescription("Streams an AI-generated summary of the markdown context file as Server-Sent Events. Pass an instruction such as 'bullet points' or 'one paragraph' to control formatting.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
