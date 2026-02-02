using AI.Common.Core;
using AiOrchestration.ValueObjects;
using CodeDebug.Data;
using CodeDebug.Models;
using CodeDebug.ValueObjects;
using Microsoft.Extensions.AI;

namespace CodeDebug.Features.AnalyzeCode.V1;


internal class AnalyzeCodeHandler : ICommandHandler<AnalyzeCodeCommand, AnalyzeCodeCommandResult>
{
    private readonly CodeDebugDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public AnalyzeCodeHandler(CodeDebugDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<AnalyzeCodeCommandResult> Handle(AnalyzeCodeCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Code, nameof(request.Code));

        // 1. Prepare AI Prompt
        var prompt = $"Analyze the following {request.Language} code for bugs and potential issues. Return a summary of issues found and a count of distinct issues.\n\nCode:\n```{request.Language}\n{request.Code}\n```";

        // 2. Call AI
        // Ideally we would want a structured response (JSON) to easily parse Issue Count.
        // For now, simple text prompt and parsing or assumption.
        // Or using ChatOptions to request JSON format if supported (middleware might handle it, or we hint in prompt).
        var aiPrompt = new List<ChatMessage>
        {
             new(ChatRole.System, "You are an expert code debugger. Provide your analysis in the following format:\nSummary: <summary text>\nIssues: <n>"),
             new(ChatRole.User, prompt)
        };

        var completion = await _chatClient.CompleteAsync(aiPrompt, cancellationToken: cancellationToken);
        var responseText = completion.Message.Text ?? "No analysis generated.";

        // 3. Parse Response (Naive)
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

        // 4. Persist
        // Create a new session for this analysis or generic session management? 
        // For standalone analysis, we create a new session.
        var sessionId = CodeDebugId.Of(Guid.NewGuid());
        // Mock user
        var userId = UserId.Of(Guid.NewGuid());
        var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "debug-model");
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

        return new AnalyzeCodeResult(sessionId.Value, reportId.Value, responseText, issueCountVal);
    }
}
