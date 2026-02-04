using AI.Common.Core;
using AiOrchestration.ValueObjects;
using Microsoft.Extensions.AI;
using Summary.Data;
using Summary.Models;
using Summary.ValueObjects;
using Ardalis.GuardClauses;
using AiOrchestration.Services;

namespace Summary.Features.SummarizeText.V1;


internal class SummarizeTextWithAIHandler : ICommandHandler<SummarizeTextCommand, SummarizeTextCommandResult>
{
    private readonly SummaryDbContext _dbContext;
    private readonly IAiOrchestrator _chatClient;

    public SummarizeTextWithAIHandler(SummaryDbContext dbContext, IAiOrchestrator chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<SummarizeTextCommandResult> Handle(SummarizeTextCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Text, nameof(request.Text));

        var prompt = $"Please provide a {request.DetailLevel} summary of the following text in {request.Language}:\n\n{request.Text}";

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are a professional summarization assistant."),
            new ChatMessage(ChatRole.User, prompt)
        };

        // Use chatClient to get the best client
        var chatClient = await _chatClient.GetClientAsync(cancellationToken: cancellationToken);
        var completion = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        var summaryText = completion.Messages[0].Text ?? "Summary generation failed.";

        // Persist
        var sessionId = SummaryId.Of(Guid.NewGuid());
        var userId = UserId.Of(Guid.NewGuid()); // Mock
        var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "summary-model");
        var config = new TextSummaryConfiguration(request.DetailLevel, LanguageCode.Of(request.Language));

        var session = TextSummarySession.Create(sessionId, userId, modelId, config);

        var resultId = SummaryResultId.Of(Guid.NewGuid());
        var summaryVo = SummaryText.Of(summaryText);
        var tokenCountVo = TokenCount.Of(completion.Usage?.TotalTokenCount ?? 0);
        var costVo = CostEstimate.Of(0);

        var result = TextSummaryResult.Create(resultId, request.Text, summaryVo, tokenCountVo, costVo);
        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SummarizeTextWithAICommandResult(sessionId.Value, resultId.Value, summaryText);
    }
}
