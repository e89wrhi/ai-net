using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using LearningAssistant.Data;
using LearningAssistant.Enums;
using LearningAssistant.Models;
using LearningAssistant.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace LearningAssistant.Features.GenerateAIQuiz.V1;

public record GenerateAIQuizCommand(string Topic, int QuestionCount) : ICommand<GenerateAIQuizResult>;

public record GenerateAIQuizResult(Guid SessionId, Guid ActivityId, string QuizContent);

public class GenerateAIQuizEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/assistant/generate-quiz",
                async (GenerateAIQuizRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new GenerateAIQuizCommand(request.Topic, request.QuestionCount);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new GenerateAIQuizResponseDto(result.SessionId, result.ActivityId, result.QuizContent));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GenerateAIQuiz")
            .WithApiVersionSet(builder.NewApiVersionSet("Assistant").Build())
            .Produces<GenerateAIQuizResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Generate AI Quiz")
            .WithDescription("Uses AI to generate a quiz on a topic.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public record GenerateAIQuizRequestDto(string Topic, int QuestionCount);
public record GenerateAIQuizResponseDto(Guid SessionId, Guid ActivityId, string QuizContent);

internal class GenerateAIQuizHandler : ICommandHandler<GenerateAIQuizCommand, GenerateAIQuizResult>
{
    private readonly LearningDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public GenerateAIQuizHandler(LearningDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<GenerateAIQuizResult> Handle(GenerateAIQuizCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Topic, nameof(request.Topic));

        var systemPrompt = $"You are a quiz master. Create a {request.QuestionCount} question quiz about {request.Topic}. Format it as JSON or clearly structured text.";
        
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, "Generate the quiz now.")
        };

        var completion = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);
        var quizText = completion.Message.Text ?? "Failed to generate quiz.";

        // Persist
        var sessionId = LearningId.Of(Guid.NewGuid());
        var userId = UserId.Of(Guid.NewGuid());
        var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "quiz-model");
        var config = LearningConfiguration.Of("assessment");

        var session = LearningSession.Create(sessionId, userId, modelId, config);

        var activityId = ActivityId.Of(Guid.NewGuid());
        var topicVo = LearningTopic.Of(request.Topic);
        var contentVo = LearningContent.Of(quizText);
        var outcomeVo = LearningOutcome.Of("Quiz Generated");
        var tokenCountVo = TokenCount.Of(completion.Usage?.TotalTokenCount ?? 0);
        var costVo = CostEstimate.Of(0);

        var activity = LearningActivity.Create(activityId, topicVo, contentVo, outcomeVo, tokenCountVo, costVo);
        session.AddActivity(activity);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new GenerateAIQuizResult(sessionId.Value, activityId.Value, quizText);
    }
}
