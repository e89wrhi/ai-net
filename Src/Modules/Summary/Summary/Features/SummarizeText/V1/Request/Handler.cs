using AI.Common.Core;
using AiOrchestration.ValueObjects;
using Microsoft.Extensions.AI;
using Summary.Data;
using Summary.Models;
using Summary.ValueObjects;

namespace Summary.Features.SummarizeText.V1;


internal class SummarizeTextWithAIHandler : ICommandHandler<SummarizeTextWithAICommand, SummarizeTextWithAICommandResult>
{
    private readonly SummaryDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public SummarizeTextWithAIHandler(SummaryDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<SummarizeTextWithAICommandResult> Handle(SummarizeTextWithAICommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Text, nameof(request.Text));

        var prompt = $"Please provide a {request.DetailLevel} summary of the following text in {request.Language}:\n\n{request.Text}";

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are a professional summarization assistant."),
            new ChatMessage(ChatRole.User, prompt)
        };

        var completion = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);
        var summaryText = completion.Message.Text ?? "Summary generation failed.";

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

        return new SummarizeTextWithAIResult(sessionId.Value, resultId.Value, summaryText);
    }
}
