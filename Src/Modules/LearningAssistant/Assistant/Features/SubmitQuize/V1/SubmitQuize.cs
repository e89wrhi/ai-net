using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using LearningAssistant.Data;
using LearningAssistant.ValueObjects;
using Duende.IdentityServer.EntityFramework.Entities;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MassTransit;
using MassTransit.Contracts;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LearningAssistant.Features.SubmitQuize.V1;


public record SubmitQuizeCommand() : ICommand<SubmitQuizeCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record SubmitQuizeCommandResponse(Guid Id);

public record SubmitQuizeRequest();

public record SubmitQuizeRequestResponse(Guid Id);

public class SubmitQuizeEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/assistant", async (SubmitQuizeRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<SubmitQuizeCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<SubmitQuizeRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("SubmitQuize")
            .WithApiVersionSet(builder.NewApiVersionSet("Assistant").Build())
            .Produces<SubmitQuizeRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Submit Quize")
            .WithDescription("Submit Quize")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class SubmitQuizeCommandValidator : AbstractValidator<SubmitQuizeCommand>
{
    public SubmitQuizeCommandValidator()
    {
    }
}

internal class SubmitQuizeHandler : IRequestHandler<SubmitQuizeCommand, SubmitQuizeCommandResponse>
{
    private readonly AssistantDbContext _dbContext;

    public SubmitQuizeHandler(AssistantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SubmitQuizeCommandResponse> Handle(SubmitQuizeCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        await _dbContext.SaveChangesAsync(cancellationToken);
        return new SubmitQuizeCommandResponse(newAssistant.Id);
    }
}
