using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace SimplePlugin.Features.StreamGenerateResponse.V1;

public class StreamGenerateResponseEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/plugin/stream_functional",
                async (StreamGenerateResponseRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                        return Results.Unauthorized();

                    var httpContext = httpContextAccessor.HttpContext!;
                    var command = new StreamGenerateResponseCommand(userId, request.Text, request.ModelId);
                    var result = await mediator.Send(command, cancellationToken);

                    httpContext.Response.ContentType = "text/event-stream";
                    httpContext.Response.Headers.CacheControl = "no-cache";
                    httpContext.Response.Headers.Connection = "keep-alive";

                    var meta = System.Text.Json.JsonSerializer.Serialize(
                        new StreamGenerateResponseResponseDto(result.ModelId, result.ProviderName));
                    await httpContext.Response.WriteAsync($"event: meta\ndata: {meta}\n\n", cancellationToken);

                    await foreach (var chunk in result.TextStream.WithCancellation(cancellationToken))
                    {
                        var escaped = chunk.Replace("\n", "\\n");
                        await httpContext.Response.WriteAsync($"data: {escaped}\n\n", cancellationToken);
                        await httpContext.Response.Body.FlushAsync(cancellationToken);
                    }

                    await httpContext.Response.WriteAsync("event: done\ndata: {}\n\n", cancellationToken);
                    return Results.Empty;
                })
            .RequireAuthorization(nameof(Duende.IdentityServer.EntityFramework.Entities.ApiScope))
            .WithName("StreamGenerateFunctionalResponse")
            .WithApiVersionSet(builder.NewApiVersionSet("FunctionalCall").Build())
            .Produces(StatusCodes.Status200OK, contentType: "text/event-stream")
            .ProducesProblem(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Stream AI Functions calling")
            .WithDescription("Streams AI tool calling execution results via SSE.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
