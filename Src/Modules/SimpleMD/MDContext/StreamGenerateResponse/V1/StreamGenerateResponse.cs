using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;
using System.Text;

namespace SimpleMD.Features.StreamGenerateResponse.V1;

public class StreamGenerateResponseEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/md/stream_chat",
                async (StreamGenerateResponseRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                        return Results.Unauthorized();

                    var httpContext = httpContextAccessor.HttpContext!;
                    var command = new StreamGenerateResponseCommand(userId, request.Text, request.ModelId);
                    var result = await mediator.Send(command, cancellationToken);

                    // Write Server-Sent Events directly to the response.
                    httpContext.Response.ContentType = "text/event-stream";
                    httpContext.Response.Headers.CacheControl = "no-cache";
                    httpContext.Response.Headers.Connection = "keep-alive";

                    // First event: model metadata as JSON.
                    var meta = System.Text.Json.JsonSerializer.Serialize(
                        new StreamGenerateResponseResponseDto(result.ModelId, result.ProviderName));
                    await httpContext.Response.WriteAsync($"event: meta\ndata: {meta}\n\n", cancellationToken);

                    // Subsequent events: streamed text chunks.
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
            .WithName("StreamGenerateMDContextResponse")
            .WithApiVersionSet(builder.NewApiVersionSet("SimpleMD").Build())
            .Produces(StatusCodes.Status200OK, contentType: "text/event-stream")
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Stream Generate MD Context Response")
            .WithDescription("Streams an AI-generated response using the markdown context file as background knowledge. Chunks arrive as Server-Sent Events.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
