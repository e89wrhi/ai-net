using AI.Common.Core;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using AiOrchestration.Services;
using CodeDebug.Data;
using CodeDebug.Models;
using CodeDebug.ValueObjects;
using Microsoft.Extensions.AI;

namespace CodeDebug.Features.AnalyzeCode.V1;


internal class AnalyzeCodeHandler : ICommandHandler<AnalyzeCodeCommand, AnalyzeCodeCommandResult>
{
    private readonly CodeDebugDbContext _dbContext;
    private readonly IAiOrchestrator _chatClient;

    public AnalyzeCodeHandler(CodeDebugDbContext dbContext, IAiOrchestrator chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<AnalyzeCodeCommandResult> Handle(AnalyzeCodeCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Code, nameof(request.Code));

        // Prepare AI Prompt
        var prompt = $"Analyze the following {request.Language} code for bugs and potential issues. Return a summary of issues found and a count of distinct issues.\n\nCode:\n```{request.Language}\n{request.Code}\n```";

        // Call AI
        // Ideally we would want a structured response (JSON) to easily parse Issue Count.
        // For now, simple text prompt and parsing or assumption.
        // Or using ChatOptions to request JSON format if supported (middleware might handle it, or we hint in prompt).
        var aiPrompt = new List<ChatMessage>
        {
             new(ChatRole.System, "You are an expert code debugger. Provide your analysis in the following format:\nSummary: <summary text>\nIssues: <n>"),
             new(ChatRole.User, prompt)
        };

        // Use chatClient to get the best client
        var chatClient = await _chatClient.GetClientAsync(cancellationToken: cancellationToken);

        var completion = await chatClient.GetResponseAsync(aiPrompt, cancellationToken: cancellationToken);
        var responseText = completion.Messages[0].Text ?? "No analysis generated.";

        // Parse Response (Naive)
        var summary = responseText;
        int issueCountVal = 1; // Default

        // Simple heuristic extraction
        var lines = responseText.Split('\n');
        var countLine = lines.FirstOrDefault(l => l.Contains("Issues:", StringComparison.OrdinalIgnoreCase));
        if (countLine != null)
        {
            var parts = countLine.Split(':');
            if (parts.Length > 1 && int.TryParse(parts[1].Trim(), out var count))
            {
                issueCountVal = count;
            }
        }

        // Persist
        // Create a new session for this analysis or generic session management? 
        // For standalone analysis, we create a new session.
        var sessionId = CodeDebugId.Of(Guid.NewGuid());
        // Mock user
        var userId = UserId.Of(Guid.NewGuid());
        var modelId = ModelId.Of(chatClient.Metadata.ModelId ?? "debug-model");
        var config = CodeDebugConfiguration.Of("standard");

        var session = CodeDebugSession.Create(sessionId, userId, modelId, config);

        var reportId = CodeDebugReportId.Of(Guid.NewGuid());
        var codeVo = SourceCode.Of(request.Code);
        var summaryVo = DebugSummary.Of(responseText); // Store full analysis as summary for now
        var issueCountVo = IssueCount.Of(issueCountVal);
        var tokenCountVo = TokenCount.Of(completion.Usage?.TotalTokenCount ?? 0);
        var costVo = CostEstimate.Of(0);

        var report = CodeDebugReport.Create(reportId, codeVo, request.Language, summaryVo, issueCountVo, tokenCountVo, costVo);

        session.AddReport(report);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AnalyzeCodeCommandResult(sessionId.Value, reportId.Value, responseText, issueCountVal);
    }
}
