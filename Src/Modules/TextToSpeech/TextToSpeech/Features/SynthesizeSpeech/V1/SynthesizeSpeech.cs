using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using TextToSpeech.Data;
using TextToSpeech.Models;
using TextToSpeech.ValueObjects;
using TextToSpeech.Enums;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace TextToSpeech.Features.SynthesizeSpeech.V1;

public class SynthesizeSpeechEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/speech/synthesize",
                async (SynthesizeSpeechRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new SynthesizeSpeechCommand(request.Text, request.Voice, request.Speed, request.Language);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new SynthesizeSpeechResponseDto(result.SessionId, result.ResultId, result.AudioUrl));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("SynthesizeSpeech")
            .WithApiVersionSet(builder.NewApiVersionSet("TextToSpeech").Build())
            .Produces<SynthesizeSpeechResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Synthesize Text to Speech")
            .WithDescription("Uses AI to convert written text into high-quality spoken audio.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
