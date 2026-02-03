using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace LearningAssistant.Features.SubmitQuiz.V1;

public class SubmitQuizEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/assistant/quiz/submit", async (SubmitQuizRequest request,
                IMediator mediator, IHttpContextAccessor httpContextAccessor, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            // current user id
            var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var command = mapper.Map<SubmitQuizCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<SubmitQuizRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("SubmitQuiz")
            .WithApiVersionSet(builder.NewApiVersionSet("Assistant").Build())
            .Produces<SubmitQuizRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Submit Quiz")
            .WithDescription("Submit Quiz")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
