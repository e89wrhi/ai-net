using AiOrchestration.ValueObjects;
using MediatR;
using Microsoft.Extensions.AI;
using Summary.Data;
using Summary.Models;
using Summary.ValueObjects;
using System.Runtime.CompilerServices;
using Ardalis.GuardClauses;
using AiOrchestration.Services;
using System.Text;

namespace Summary.Features.StreamSummarizeText.V1;


internal class StreamSummarizeTextHandler : IStreamRequestHandler<StreamSummarizeTextCommand, string>
{
    private readonly SummaryDbContext _dbContext;
    private readonly IAiOrchestrator _chatClient;

    public StreamSummarizeTextHandler(SummaryDbContext dbContext, IAiOrchestrator chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async IAsyncEnumerable<string> Handle(StreamSummarizeTextCommand request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Text, nameof(request.Text));

        var prompt = $"Please provide a {request.DetailLevel} summary of the following text in {request.Language}:\n\n{request.Text}";
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are a professional summarization assistant."),
            new ChatMessage(ChatRole.User, prompt)
        };

        var fullSummaryBuilder = new StringBuilder();
        int tokenEstimate = 0;

        // Use chatClient to get the best client
        var chatClient = await _chatClient.GetClientAsync(cancellationToken: cancellationToken);
        var response = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        foreach (var update in response.Messages)
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                fullSummaryBuilder.Append(update.Text);
                tokenEstimate++;
                yield return update.Text;
            }
        }

        // Persist session after stream completion
        await PersistSummaryAsync(request, fullSummaryBuilder.ToString(), tokenEstimate, cancellationToken);
    }

    private async Task PersistSummaryAsync(StreamSummarizeTextCommand request, string fullSummary, int tokenUsage, CancellationToken cancellationToken)
    {
        try
        {
            var sessionId = SummaryId.Of(Guid.NewGuid());
            var userId = UserId.Of(Guid.NewGuid());
            var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "summary-stream-model");
            var config = new TextSummaryConfiguration(request.DetailLevel, LanguageCode.Of(request.Language));

            var session = TextSummarySession.Create(sessionId, userId, modelId, config);

            var resultId = SummaryResultId.Of(Guid.NewGuid());
            var summaryVo = SummaryText.Of(fullSummary);
            var tokenCountVo = TokenCount.Of(tokenUsage);
            var costVo = CostEstimate.Of(0);

            var result = TextSummaryResult.Create(resultId, request.Text, summaryVo, tokenCountVo, costVo);
            session.AddResult(result);

            _dbContext.Sessions.Add(session);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // Log persistence error
        }
    }
}
