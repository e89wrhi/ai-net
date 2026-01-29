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
using LearningAssistant.Enums;
using LearningAssistant.Models;

namespace LearningAssistant.Features.GenerateLesson.V1;


public record GenerateLessonCommand(Guid ProfileId, string Title, string Content, DifficultyLevel Level) : ICommand<GenerateLessonCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record GenerateLessonCommandResponse(Guid Id);

public record GenerateLessonRequest(Guid ProfileId, string Title, string Content, DifficultyLevel Level);

public record GenerateLessonRequestResponse(Guid Id);

public class GenerateLessonEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/assistant/lesson", async (GenerateLessonRequest request,
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
        RuleFor(x => x.ProfileId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Content).NotEmpty();
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

        var lesson = LessonModel.Create(
            LessonId.Of(NewId.NextGuid()),
            ProfileId.Of(request.ProfileId),
            request.Title,
            request.Content,
            request.Level);

        await _dbContext.Lessons.AddAsync(lesson, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new GenerateLessonCommandResponse(lesson.Id.Value);
    }
}

