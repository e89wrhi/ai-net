using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using CodeDebug.Data;
using CodeDebug.Models;
using CodeDebug.ValueObjects;
using Microsoft.Extensions.AI;
using System.Configuration.Provider;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CodeDebug.Features.AnalyzeCode.V1;


internal class AnalyzeCodeHandler : ICommandHandler<AnalyzeCodeCommand, AnalyzeCodeCommandResult>
{
    private readonly CodeDebugDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IChatClient _chatClient;
    private readonly IAiModelService _modelService;

    public AnalyzeCodeHandler(CodeDebugDbContext dbContext, IAiModelService modelService, IAiOrchestrator orchestrator, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _modelService = modelService;
        _orchestrator = orchestrator;
        _chatClient = chatClient;
    }

    public async Task<AnalyzeCodeCommandResult> Handle(AnalyzeCodeCommand request, CancellationToken cancellationToken)
    {
        #region Prompt
        var messages = new List<ChatMessage>
        {
             new ChatMessage(
                    role: ChatRole.System, 
                    content: "You are an expert code debugger. Analyze the provided code for bugs, security vulnerabilities, and logic issues. " +
            "You MUST respond with a VALID JSON object containing 'summary' (string) and 'issueCount' (integer). " +
            "Example: { \"summary\": \"Found a null reference\", \"issueCount\": 1 }"),
             new ChatMessage(
                    role: ChatRole.User, 
                    content: $"Analyze this {request.Language} code:\n\n```{request.Language}\n{request.Code}\n```")
        };
        #endregion

        Guard.Against.NullOrEmpty(request.Code, nameof(request.Code));

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
        //  user
        var userId = UserId.Of(request.UserId);
        var config = new CodeDebugConfiguration(
            depth: Enums.DebugDepth.Surface,
            includeSuggestions: false);

        var session = CodeDebugSession.Create(sessionId, userId, modelId, config);

        var reportId = CodeDebugReportId.Of(Guid.NewGuid());
        var codeVo = SourceCode.Of(request.Code);
        var summaryVo = DebugSummary.Of(analysis.Summary); 
        var issueCountVo = IssueCount.Of(analysis.IssueCount);
        var tokenCountVo = TokenCount.Of(tokenUsage);
        var costVo = CostEstimate.Of(costValue);

        var report = CodeDebugReport.Create(
                    reportId, 
                    codeVo, 
                    request.Language, 
                    summaryVo, 
                    issueCountVo, 
                    tokenCountVo, 
                    costVo);

        session.AddReport(report);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AnalyzeCodeCommandResult(sessionId.Value, reportId.Value, 
            analysis.Summary, analysis.IssueCount, modelIdStr, providerName);
    }

    private record AiAnalysisResult(string Summary, int IssueCount);
}
