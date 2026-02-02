using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace TextToSpeech.Features.GenerateAudio.V1;

public class GenerateAudioEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/speech/generate-ai",
                async (GenerateAudioWithAIRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new GenerateAudioWithAICommand(request.Text, request.Voice);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new GenerateAudioWithAIResponseDto(result.SessionId, result.AudioUrl));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GenerateAudio")
            .WithApiVersionSet(builder.NewApiVersionSet("TextToSpeech").Build())
            .Produces<GenerateAudioWithAIResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Generate AI Voice")
            .WithDescription("Uses AI to generate expressive spoken audio from text with selection of voice types.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
