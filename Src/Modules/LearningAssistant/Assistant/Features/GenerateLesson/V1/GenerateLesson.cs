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

namespace LearningAssistant.Features.GenerateLesson.V1;


public record GenerateLessonCommand() : ICommand<GenerateLessonCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record GenerateLessonCommandResponse(Guid Id);

public record GenerateLessonRequest();

public record GenerateLessonRequestResponse(Guid Id);

public class GenerateLessonEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/assistant", async (GenerateLessonRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<GenerateLessonCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<GenerateLessonRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GenerateLesson")
            .WithApiVersionSet(builder.NewApiVersionSet("Assistant").Build())
            .Produces<GenerateLessonRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Generate Lesson")
            .WithDescription("Generate Lesson")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class GenerateLessonCommandValidator : AbstractValidator<GenerateLessonCommand>
{
    public GenerateLessonCommandValidator()
    {
    }
}

internal class GenerateLessonHandler : IRequestHandler<GenerateLessonCommand, GenerateLessonCommandResponse>
{
    private readonly AssistantDbContext _dbContext;

    public GenerateLessonHandler(AssistantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GenerateLessonCommandResponse> Handle(GenerateLessonCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        await _dbContext.SaveChangesAsync(cancellationToken);
        return new GenerateLessonCommandResponse(newImage.Id);
    }
}
