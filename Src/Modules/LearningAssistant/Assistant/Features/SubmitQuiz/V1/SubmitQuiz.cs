using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using LearningAssistant.Data;
using LearningAssistant.ValueObjects;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using LearningAssistant.Exceptions;
using Microsoft.EntityFrameworkCore;
using LearningAssistant.Models;
using Duende.IdentityServer.EntityFramework.Entities;

namespace LearningAssistant.Features.SubmitQuiz.V1;

public class SubmitQuizEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/assistant/quiz/submit", async (SubmitQuizRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<SubmitQuizCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<SubmitQuizRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("SubmitQuiz")
            .WithApiVersionSet(builder.NewApiVersionSet("Assistant").Build())
            .Produces<SubmitQuizRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Submit Quiz")
            .WithDescription("Submit Quiz")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
