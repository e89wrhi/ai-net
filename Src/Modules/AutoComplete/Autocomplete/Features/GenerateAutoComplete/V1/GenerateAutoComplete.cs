using System.Security.Claims;
using AI.Common.Web;
using AutoComplete.Enums;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AutoComplete.Features.GenerateAutoComplete.V1;

public class GenerateAutoCompleteEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/autocomplete/generate",
                async (GenerateAutoCompleteRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    
                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new GenerateAutoCompleteCommand(userId, request.Prompt, request.Mode, request.ModelId);
                    var result = await mediator.Send(command, cancellationToken);

                    var response = new GenerateAutoCompleteResponseDto(result.Completion, result.TokensUsed, 
                        result.EstimatedCost, 
                        result.ModelId, result.ProviderName);

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GenerateAutoComplete")
            .WithApiVersionSet(builder.NewApiVersionSet("AutoComplete").Build())
            .Produces<GenerateAutoCompleteResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Generate Auto Complete")
            .WithDescription("Generates text completion using an AI model and stores the interaction history.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
