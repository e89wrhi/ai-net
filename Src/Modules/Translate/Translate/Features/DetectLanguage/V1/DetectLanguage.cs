using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Translate.Features.DetectLanguage.V1;

public class DetectLanguageEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/translate/detect",
                async (DetectLanguageRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new DetectLanguageCommand(request.Text);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new DetectLanguageResponseDto(result.DetectedLanguageCode, result.Confidence));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("DetectLanguage")
            .WithApiVersionSet(builder.NewApiVersionSet("Translate").Build())
            .Produces<DetectLanguageResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Detect Language")
            .WithDescription("Uses AI to detect the language of the provided text.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
