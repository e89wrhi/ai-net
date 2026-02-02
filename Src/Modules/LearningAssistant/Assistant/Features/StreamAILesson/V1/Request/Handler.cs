using AiOrchestration.ValueObjects;
using LearningAssistant.Data;
using LearningAssistant.Models;
using LearningAssistant.ValueObjects;
using MediatR;
using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;
using System.Text;

namespace LearningAssistant.Features.StreamAILesson.V1;

internal class StreamAILessonHandler : IStreamRequestHandler<StreamAILessonCommand, string>
{
    private readonly LearningDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public StreamAILessonHandler(LearningDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async IAsyncEnumerable<string> Handle(StreamAILessonCommand request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Topic, nameof(request.Topic));

        var systemPrompt = $"You are a professional teacher. Generate a comprehensive lesson about the topic provided for a {request.Level} level student.";
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, $"Explain: {request.Topic}")
        };

        var fullContentBuilder = new StringBuilder();
        int tokenEstimate = 0;

        await foreach (var update in _chatClient.CompleteStreamingAsync(messages, cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                fullContentBuilder.Append(update.Text);
                tokenEstimate++;
                yield return update.Text;
            }
        }

        // Persist interaction
        await PersistActivityAsync(request, fullContentBuilder.ToString(), tokenEstimate, cancellationToken);
    }

    private async Task PersistActivityAsync(StreamAILessonCommand request, string fullContent, int tokenUsage, CancellationToken cancellationToken)
    {
        try
        {
            var sessionId = LearningId.Of(Guid.NewGuid());
            var userId = UserId.Of(Guid.NewGuid());
            var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "learning-stream-model");
            var config = LearningConfiguration.Of("streaming");

            var session = LearningSession.Create(sessionId, userId, modelId, config);

            var activityId = ActivityId.Of(Guid.NewGuid());
            var topicVo = LearningTopic.Of(request.Topic);
            var contentVo = LearningContent.Of(fullContent);
            var outcomeVo = LearningOutcome.Of("Streamed Lesson Completed");
            var tokenCountVo = TokenCount.Of(tokenUsage);
            var costVo = CostEstimate.Of(0);

            var activity = LearningActivity.Create(activityId, topicVo, contentVo, outcomeVo, tokenCountVo, costVo);
            session.AddActivity(activity);

            _dbContext.Sessions.Add(session);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // Log error
        }
    }
}

