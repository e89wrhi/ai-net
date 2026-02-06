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
    private readonly IAiOrchestrator _orchestrator;
    private readonly IAiModelService _modelService;

    public StreamSummarizeTextHandler(SummaryDbContext dbContext, IAiOrchestrator orchestrator, IAiModelService modelService)
    {
        _dbContext = dbContext;
        _orchestrator = orchestrator;
        _modelService = modelService;
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

        // Use orchestrator to get the client based on requested model criteria
        var criteria = new ModelCriteria { ModelId = request.ModelId };
        var chatClient = await _orchestrator.GetClientAsync(criteria, cancellationToken);
        
        await foreach (var update in chatClient.GetStreamingResponseAsync(messages, cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                fullSummaryBuilder.Append(update.Text);
                tokenEstimate++;
                yield return update.Text;
            }
        }

        // Persist session after stream completion
        await PersistSummaryAsync(request, fullSummaryBuilder.ToString(), tokenEstimate, chatClient, cancellationToken);
    }

    private async Task PersistSummaryAsync(StreamSummarizeTextCommand request, string fullSummary, int tokenUsage, IChatClient chatClient, CancellationToken cancellationToken)
    {
        try
        {
            var sessionId = SummaryId.Of(Guid.NewGuid());
            var userId = UserId.Of(request.UserId);

            
            var clientMetadata = chatClient.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata;
            var modelIdStr = clientMetadata?.DefaultModelId ?? "summary-stream-model";
            var modelId = ModelId.Of(modelIdStr);
            
            var config = new TextSummaryConfiguration(request.DetailLevel, LanguageCode.Of(request.Language));

            var session = TextSummarySession.Create(sessionId, userId, modelId, config);

            var resultId = SummaryResultId.Of(Guid.NewGuid());
            var summaryVo = SummaryText.Of(fullSummary);
            var tokenCountVo = TokenCount.Of(tokenUsage);
            
            // Get cost from model service
            var costPerToken = _modelService.GetCostPerToken(modelId);
            var costValue = (decimal)tokenUsage * costPerToken;
            var costVo = CostEstimate.Of(costValue);

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

