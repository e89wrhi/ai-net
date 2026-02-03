using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Translate.Features.StreamTranslateText.V1;

public class StreamTranslateTextEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/translate/translate-stream",
                (StreamTranslateTextRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    return mediator.CreateStream(new StreamTranslateTextCommand(request.Text, request.SourceLanguage, request.TargetLanguage, request.DetailLevel), cancellationToken);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StreamTranslateText")
            .WithApiVersionSet(builder.NewApiVersionSet("Translate").Build())
            .Produces<IAsyncEnumerable<string>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Stream Text Translation")
            .WithDescription("Streams the translated text from a source language to a target language.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
