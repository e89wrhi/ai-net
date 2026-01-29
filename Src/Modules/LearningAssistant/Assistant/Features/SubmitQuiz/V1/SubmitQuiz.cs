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

namespace LearningAssistant.Features.SubmitQuiz.V1;


public record SubmitQuizCommand(Guid LessonId, Guid QuizId, double Score) : ICommand<SubmitQuizCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record SubmitQuizCommandResponse(Guid Id);

public record SubmitQuizRequest(Guid LessonId, Guid QuizId, double Score);

public record SubmitQuizRequestResponse(Guid Id);

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

public class SubmitQuizCommandValidator : AbstractValidator<SubmitQuizCommand>
{
    public SubmitQuizCommandValidator()
    {
        RuleFor(x => x.LessonId).NotEmpty();
        RuleFor(x => x.QuizId).NotEmpty();
    }
}

internal class SubmitQuizHandler : IRequestHandler<SubmitQuizCommand, SubmitQuizCommandResponse>
{
    private readonly AssistantDbContext _dbContext;

    public SubmitQuizHandler(AssistantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SubmitQuizCommandResponse> Handle(SubmitQuizCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var lesson = await _dbContext.Lessons
            .FirstOrDefaultAsync(x => x.Id == LessonId.Of(request.LessonId), cancellationToken);

        if (lesson == null)
        {
            throw new LessonNotFoundException(request.LessonId);
        }

        var profile = await _dbContext.Profiles
            .Include(x => x.Lessons)
                .ThenInclude(l => l.Quizzes)
            .FirstOrDefaultAsync(x => x.Id == lesson.ProfileId, cancellationToken);

        if (profile == null)
        {
            throw new ProfileNotFoundException(lesson.ProfileId);
        }

        profile.SubmitQuiz(LessonId.Of(request.LessonId), QuizId.Of(request.QuizId), request.Score);

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new SubmitQuizCommandResponse(request.QuizId);
    }
}
