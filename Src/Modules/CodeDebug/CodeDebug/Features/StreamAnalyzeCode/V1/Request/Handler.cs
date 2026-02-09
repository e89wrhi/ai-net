using AiOrchestration.ValueObjects;
using CodeDebug.Data;
using CodeDebug.Models;
using CodeDebug.ValueObjects;
using MediatR;
using Microsoft.Extensions.AI;
using Ardalis.GuardClauses;
using AiOrchestration.Services;
using System.Runtime.CompilerServices;
using System.Text;
using AiOrchestration.Models;

namespace CodeDebug.Features.StreamAnalyzeCode.V1;

internal class StreamAnalyzeCodeHandler : IStreamRequestHandler<StreamAnalyzeCodeCommand, string>
{
    private readonly CodeDebugDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IAiModelService _modelService;

    public StreamAnalyzeCodeHandler(CodeDebugDbContext dbContext, IAiOrchestrator orchestrator, IAiModelService modelService)
    {
        _dbContext = dbContext;
        _orchestrator = orchestrator;
        _modelService = modelService;
    }

    public async IAsyncEnumerable<string> Handle(StreamAnalyzeCodeCommand request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Code, nameof(request.Code));

        var prompt = $"Analyze the following {request.Language} code for bugs. Provide a detailed report.\n\nCode:\n{request.Code}";
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are an expert debugger. Provide a detailed analysis report."),
            new ChatMessage(ChatRole.User, prompt)
        };

        var fullReportBuilder = new StringBuilder();
        int tokenEstimate = 0;

        // Use orchestrator to get the client based on requested model criteria
        var criteria = new ModelCriteria { ModelId = request.ModelId };
        var chatClient = await _orchestrator.GetClientAsync(criteria, cancellationToken);
        
        await foreach (var update in chatClient.GetStreamingResponseAsync(messages, cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                fullReportBuilder.Append(update.Text);
                tokenEstimate++;
                yield return update.Text;
            }
        }

        // Persist
        await PersistReportAsync(request, fullReportBuilder.ToString(), tokenEstimate, chatClient, cancellationToken);
    }

    private async Task PersistReportAsync(StreamAnalyzeCodeCommand request, string fullReport, int tokenUsage, IChatClient chatClient, CancellationToken cancellationToken)
    {
        try
        {
            var sessionId = CodeDebugId.Of(Guid.NewGuid());
            var userId = UserId.Of(request.UserId);

            
            var clientMetadata = chatClient.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata;
            var modelIdStr = clientMetadata?.DefaultModelId ?? "debug-stream-model";
            var modelId = ModelId.Of(modelIdStr);
            
            var config = new CodeDebugConfiguration(
                depth: request.Depth,
                includeSuggestions: request.IncludeSuggestion);

            var session = CodeDebugSession.Create(sessionId, userId, modelId, config);

            var reportId = CodeDebugReportId.Of(Guid.NewGuid());
            var codeVo = SourceCode.Of(request.Code);
            var summaryVo = DebugSummary.Of(fullReport);
            var issueCountVo = IssueCount.Of(1); // Placeholder or parse
            var tokenCountVo = TokenCount.Of(tokenUsage);
            
            // Get cost from model service
            var costPerToken = _modelService.GetCostPerToken(modelId);
            var costValue = (decimal)tokenUsage * costPerToken;
            var costVo = CostEstimate.Of(costValue);

            var report = CodeDebugReport.Create(reportId, codeVo, request.Language, summaryVo, issueCountVo, tokenCountVo, costVo);
            session.AddReport(report);

            _dbContext.Sessions.Add(session);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // Log error
        }
    }
}


