using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using CodeDebug.Data;
using CodeDebug.Exceptions;
using CodeDebug.ValueObjects;
using Microsoft.Extensions.AI;

namespace CodeDebug.Features.GenerateFix.V1;


internal class GenerateFixHandler : ICommandHandler<GenerateFixCommand, GenerateFixCommandResult>
{
    private readonly CodeDebugDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IChatClient _chatClient;
    private readonly IAiModelService _modelService;

    public GenerateFixHandler(CodeDebugDbContext dbContext, IAiModelService modelService, IAiOrchestrator orchestrator, IChatClient chatClient)
    {
        _modelService = modelService;
        _orchestrator = orchestrator;
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<GenerateFixCommandResult> Handle(GenerateFixCommand request, CancellationToken cancellationToken)
    {
        #region Prompt
        var messages = new List<ChatMessage>
        {
             new ChatMessage(
                    role: ChatRole.System, 
                    content: "You are an expert coder. Fix the provided code."),
             new ChatMessage(
                    role: ChatRole.User, 
                    content : $"Based on the following code and analysis, provide the fixed code. Return ONLY the code in one block, followed by a brief explanation.\n\nOriginal Code:\n{report.Code.Value}\n\nAnalysis:\n{report.Summary.Value}")
        };
        #endregion

        // Use orchestrator to get the client based on requested model criteria
        var criteria = new ModelCriteria { ModelId = request.ModelId };
        var chatClient = await _orchestrator.GetClientAsync(criteria, cancellationToken);

        // Get actual model info from client metadata
        var clientMetadata = chatClient.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata;
        var modelIdStr = clientMetadata?.DefaultModelId ?? "default-model";
        var providerName = clientMetadata?.ProviderName ?? "Unknown";
        var modelId = ModelId.Of(modelIdStr);

        // Call AI Model
        var chatCompletion = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        var responseText = chatCompletion.Messages[0].Text ?? string.Empty;

        // Calculate Metadata & Usage
        var tokenUsage = chatCompletion.Usage?.TotalTokenCount ?? (messages.Sum(i => i.Text.Length) + responseText.Length) / 4;

        // Get cost per token from model service
        var costPerToken = _modelService.GetCostPerToken(modelId);
        var costValue = (decimal)tokenUsage * costPerToken;

        // Load Session to check existence
        var session = await _dbContext.Sessions.FindAsync(new object[] { CodeDebugId.Of(request.SessionId) }, cancellationToken);
        if (session == null) throw new CodeDebugNotFoundException(request.SessionId);

        // Load Report
        // NOTE: Aggregate root should manage reports. EF Core explicit loading.
        // Or if session.Reports is loaded?
        // Assuming we need to fetch via DbContext directly if not loaded.
        // It's cleaner to query via Report Id directly if accessible or via session.
        // Let's assume Report is part of Session and check if we can access it.
        // Since `_reports` is private backing field, we trust EF loaded it if we did Include, which usage of FindAsync won't do for collections usually.

        // Alternative: Query directly (if Report was its own root, but it seems to be Entity inside Aggregate)
        var report = await _dbContext.Reports.FindAsync(new object[] { CodeDebugReportId.Of(request.ReportId) }, cancellationToken);

        if (report == null)
            throw new CodeDebugReportNotFoundException(request.ReportId);

        // Heuristic split of Code vs Explanation (assuming markdown code blocks)
        var fixedCode = response;
        var explanation = "See code changes.";

        if (response.Contains("```"))
        {
            var parts = response.Split("```", StringSplitOptions.RemoveEmptyEntries);
            // parts[0] might be intro, parts[1] code (if language tag present), parts[2] outro/explanation
            foreach (var part in parts)
            {
                // Simple assumption: the longest block or one looking like code is code. 
                // Or just the first block that is not the intro.
                var trimmed = part.Trim();
                if (!trimmed.StartsWith("Fix") && !trimmed.StartsWith("Here"))
                {
                    // Strip language tag if first line
                    var firstLineEnd = trimmed.IndexOf('\n');
                    if (firstLineEnd > 0 && firstLineEnd < 20)
                    {
                        // e.g. "csharp"
                        fixedCode = trimmed.Substring(firstLineEnd + 1);
                    }
                    else
                    {
                        fixedCode = trimmed;
                    }
                    break;
                }
            }
            // Just return full response as explanation if parsing fails or stick to full response in explanation.
            explanation = response.Replace(fixedCode, "").Replace("```", "").Trim();
        }

        // We could create a "FixedReport" or track fixes in the session.
        // For now, just return.

        return new GenerateFixCommandResult(fixedCode, explanation, modelIdStr, providerName);
    }
}
