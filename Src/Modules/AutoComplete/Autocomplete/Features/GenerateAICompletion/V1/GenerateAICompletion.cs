using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using AutoComplete.Data;
using AutoComplete.Models;
using AutoComplete.ValueObjects;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace AutoComplete.Features.GenerateAICompletion.V1;

public class GenerateAICompletionEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/autocomplete/generate",
                async (GenerateAutoCompleteRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // In a real scenario, extract UserId from Claims
                    // var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var userId = Guid.NewGuid(); // Mock User ID for now

                    var command = new GenerateAutoCompleteCommand(userId, request.Prompt);
                    var result = await mediator.Send(command, cancellationToken);

                    var response = new GenerateAutoCompleteResponseDto(result.Completion, result.TokensUsed, result.EstimatedCost);

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GenerateAICompletion")
            .WithApiVersionSet(builder.NewApiVersionSet("AutoComplete").Build())
            .Produces<GenerateAutoCompleteResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Generate AI Completion")
            .WithDescription("Generates text completion using an AI model and stores the interaction history.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
