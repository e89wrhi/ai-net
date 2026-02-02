using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Translate.Features.TranslateText.V1;

public class TranslateTextEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/translate/translate",
                async (TranslateTextWithAIRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new TranslateTextWithAICommand(request.Text, request.SourceLanguage, request.TargetLanguage, request.DetailLevel);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new TranslateTextWithAIResponseDto(result.SessionId, result.ResultId, result.TranslatedText));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("TranslateText")
            .WithApiVersionSet(builder.NewApiVersionSet("Translate").Build())
            .Produces<TranslateTextWithAIResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Translate Text")
            .WithDescription("Uses AI to translate text from a source language to a target language.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
