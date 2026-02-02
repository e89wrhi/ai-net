using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace User.Features.GenerateUserPersona.V1;

public class GenerateUserPersonaEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/user/persona",
                async (GenerateUserPersonaWithAIRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new GenerateUserPersonaWithAICommand(request.UserId);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new GenerateUserPersonaWithAIResponseDto(result.PersonaName, result.Description, result.Traits));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GenerateUserPersona")
            .WithApiVersionSet(builder.NewApiVersionSet("User").Build())
            .Produces<GenerateUserPersonaWithAIResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Generate User AI Persona")
            .WithDescription("Uses AI to categorize the user into a specific 'AI Persona' based on their tool preferences.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
