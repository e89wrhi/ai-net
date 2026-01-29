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
using Microsoft.EntityFrameworkCore;
using LearningAssistant.Exceptions;
using LearningAssistant.Models;

namespace LearningAssistant.Features.GenerateQuiz.V1;


public record GenerateQuizCommand(Guid LessonId, string Questions) : ICommand<GenerateQuizCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record GenerateQuizCommandResponse(Guid Id);

public record GenerateQuizRequest(Guid LessonId, string Questions);

public record GenerateQuizRequestResponse(Guid Id);

public class GenerateQuizEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/assistant/quiz", async (GenerateQuizRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<GenerateQuizCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<GenerateQuizRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GenerateQuiz")
            .WithApiVersionSet(builder.NewApiVersionSet("Assistant").Build())
            .Produces<GenerateQuizRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Generate Quiz")
            .WithDescription("Generate Quiz")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class GenerateQuizCommandValidator : AbstractValidator<GenerateQuizCommand>
{
    public GenerateQuizCommandValidator()
    {
        RuleFor(x => x.LessonId).NotEmpty();
        RuleFor(x => x.Questions).NotEmpty();
    }
}

internal class GenerateQuizHandler : IRequestHandler<GenerateQuizCommand, GenerateQuizCommandResponse>
{
    private readonly AssistantDbContext _dbContext;

    public GenerateQuizHandler(AssistantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GenerateQuizCommandResponse> Handle(GenerateQuizCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var lesson = await _dbContext.Lessons.FindAsync(new object[] { LessonId.Of(request.LessonId) }, cancellationToken);

        if (lesson == null)
        {
            throw new LessonNotFoundException(request.LessonId);
        }

        var profile = await _dbContext.Profiles
            .Include(x => x.Lessons)
            .FirstOrDefaultAsync(x => x.Id == lesson.ProfileId, cancellationToken);

        if (profile == null)
        {
            throw new ProfileNotFoundException(lesson.ProfileId);
        }

        var quiz = QuizModel.Create(
            QuizId.Of(NewId.NextGuid()),
            lesson.Id,
            request.Questions);

        profile.AddQuizToLesson(lesson.Id, quiz);

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new GenerateQuizCommandResponse(quiz.Id.Value);
    }
}
