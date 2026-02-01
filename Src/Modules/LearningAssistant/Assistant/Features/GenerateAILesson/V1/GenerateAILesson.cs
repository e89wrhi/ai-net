using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using LearningAssistant.Data;
using LearningAssistant.Models;
using LearningAssistant.ValueObjects;
using LearningAssistant.Enums;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace LearningAssistant.Features.GenerateAILesson.V1;

public record GenerateAILessonCommand(string Topic, DifficultyLevel Level) : ICommand<GenerateAILessonResult>;

public record GenerateAILessonResult(Guid SessionId, Guid ActivityId, string Content);

public class GenerateAILessonEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/assistant/generate-lesson",
                async (GenerateAILessonRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new GenerateAILessonCommand(request.Topic, request.Level);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new GenerateAILessonResponseDto(result.SessionId, result.ActivityId, result.Content));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GenerateAILesson")
            .WithApiVersionSet(builder.NewApiVersionSet("Assistant").Build())
            .Produces<GenerateAILessonResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Generate AI Lesson")
            .WithDescription("Uses AI to generate a lesson on a specific topic.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public record GenerateAILessonRequestDto(string Topic, DifficultyLevel Level);
public record GenerateAILessonResponseDto(Guid SessionId, Guid ActivityId, string Content);

internal class GenerateAILessonHandler : ICommandHandler<GenerateAILessonCommand, GenerateAILessonResult>
{
    private readonly LearningDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public GenerateAILessonHandler(LearningDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<GenerateAILessonResult> Handle(GenerateAILessonCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Topic, nameof(request.Topic));

        var systemPrompt = $"You are a professional teacher. Generate a comprehensive lesson about the topic provided for a {request.Level} level student.";
        
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, $"Explain: {request.Topic}")
        };

        var completion = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);
        var contentText = completion.Message.Text ?? "Failed to generate lesson.";

        // Persist
        var sessionId = LearningId.Of(Guid.NewGuid());
        var userId = UserId.Of(Guid.NewGuid()); // Mock
        var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "learning-model");
        var config = LearningConfiguration.Of("standard");

        var session = LearningSession.Create(sessionId, userId, modelId, config);

        var activityId = ActivityId.Of(Guid.NewGuid());
        var topicVo = LearningTopic.Of(request.Topic);
        var contentVo = LearningContent.Of(contentText);
        var outcomeVo = LearningOutcome.Of("Lesson Generated");
        var tokenCountVo = TokenCount.Of(completion.Usage?.TotalTokenCount ?? 0);
        var costVo = CostEstimate.Of(0);

        var activity = LearningActivity.Create(activityId, topicVo, contentVo, outcomeVo, tokenCountVo, costVo);
        session.AddActivity(activity);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new GenerateAILessonResult(sessionId.Value, activityId.Value, contentText);
    }
}
