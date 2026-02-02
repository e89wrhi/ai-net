using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using ChatBot.Data;
using ChatBot.Enums;
using ChatBot.Exceptions;
using ChatBot.Models;
using ChatBot.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace ChatBot.Features.GenerateAiResponse.V1;

public class GenerateAiResponseEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/chat/generate-response",
                async (GenerateAiResponseRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new GenerateAiResponseCommand(request.SessionId);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new GenerateAiResponseResponseDto(result.MessageId, result.Content));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GenerateAiResponse")
            .WithApiVersionSet(builder.NewApiVersionSet("Chat").Build())
            .Produces<GenerateAiResponseResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Generate AI Response")
            .WithDescription("Triggers the AI to generate a response for the given chat session.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
