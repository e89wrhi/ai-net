using AI.Common.Core;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using AiOrchestration.Services;
using CodeDebug.Data;
using CodeDebug.Models;
using CodeDebug.ValueObjects;
using Microsoft.Extensions.AI;
using System.Text.Json;
using System.Text.Json.Serialization;

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

        // Prepare AI Prompt with Structured Output instructions
        var systemInstructions = 
            "You are an expert code debugger. Analyze the provided code for bugs, security vulnerabilities, and logic issues. " +
            "You MUST respond with a VALID JSON object containing 'summary' (string) and 'issueCount' (integer). " +
            "Example: { \"summary\": \"Found a null reference\", \"issueCount\": 1 }";

        var userPrompt = $"Analyze this {request.Language} code:\n\n```{request.Language}\n{request.Code}\n```";

        var aiMessages = new List<ChatMessage>
        {
             new(ChatRole.System, systemInstructions),
             new(ChatRole.User, userPrompt)
        };

        // Use orchestrator to get the configured client
        var chatClient = await _chatClient.GetClientAsync(cancellationToken: cancellationToken);

        var completion = await chatClient.GetResponseAsync(aiMessages, cancellationToken: cancellationToken);
        var responseText = completion.Messages[0].Text ?? "{}";

        // Structured Parsing
        AiAnalysisResult analysis;
        try 
        {
            // Clean markdown if AI wrapped JSON in ```json
            var cleanJson = responseText.Contains("```json") 
                ? responseText.Split("```json")[1].Split("```")[0].Trim()
                : responseText.Trim('`', ' ', '\n', '\r');

            analysis = JsonSerializer.Deserialize<AiAnalysisResult>(cleanJson, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            }) ?? new AiAnalysisResult("Could not parse AI response.", 0);
        }
        catch
        {
            analysis = new AiAnalysisResult(responseText, 1);
        }

        // Persist
        // Create a new session for this analysis or generic session management? 
        // For standalone analysis, we create a new session.
        var sessionId = CodeDebugId.Of(Guid.NewGuid());
        // Mock user
        var userId = UserId.Of(Guid.NewGuid());
        var clientMetadata = chatClient.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata;
        var modelId = ModelId.Of(clientMetadata?.ModelId ?? "debug-model");
        var config = CodeDebugConfiguration.Of("standard");

        var session = CodeDebugSession.Create(sessionId, userId, modelId, config);

        var reportId = CodeDebugReportId.Of(Guid.NewGuid());
        var codeVo = SourceCode.Of(request.Code);
        var summaryVo = DebugSummary.Of(analysis.Summary); 
        var issueCountVo = IssueCount.Of(analysis.IssueCount);
        var tokenCountVo = TokenCount.Of(completion.Usage?.TotalTokenCount ?? 0);
        var costVo = CostEstimate.Of(0);

        var report = CodeDebugReport.Create(reportId, codeVo, request.Language, summaryVo, issueCountVo, tokenCountVo, costVo);

        session.AddReport(report);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AnalyzeCodeCommandResult(sessionId.Value, reportId.Value, analysis.Summary, analysis.IssueCount);
    }

    private record AiAnalysisResult(string Summary, int IssueCount);
}
