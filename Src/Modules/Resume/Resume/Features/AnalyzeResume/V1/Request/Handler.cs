using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using Microsoft.Extensions.AI;
using Resume.Data;
using Resume.Models;
using Resume.ValueObjects;

namespace Resume.Features.AnalyzeResume.V1;


internal class AnalyzeResumeWithAIHandler : ICommandHandler<AnalyzeResumeCommand, AnalyzeResumeCommandResult>
{
    private readonly ResumeDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IChatClient _chatClient;
    private readonly IAiModelService _modelService;

    public AnalyzeResumeWithAIHandler(ResumeDbContext dbContext, IAiModelService modelService, IAiOrchestrator orchestrator, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _modelService = modelService;
        _orchestrator = orchestrator;
        _chatClient = chatClient;
    }

    public async Task<AnalyzeResumeCommandResult> Handle(AnalyzeResumeCommand request, CancellationToken cancellationToken)
    {
        #region Prompt
        var messages = new List<ChatMessage>
        {
            new ChatMessage(
                    role: ChatRole.System, 
                    content: "You are an HR expert system. Analyze the provided resume text. Extract a summary and provide a candidate score (0-100) based on professional standards."),
            new ChatMessage(
                    role: ChatRole.User, 
                    content : $"Resume Content:\n{request.ResumeContent}")
        };
        #endregion

        Guard.Against.NullOrEmpty(request.ResumeContent, nameof(request.ResumeContent));

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
        var responseText = chatCompletion.Messages[0].Text ?? "Failed to analyze resume.";

        // Calculate Metadata & Usage
        var tokenUsage = chatCompletion.Usage?.TotalTokenCount ?? (messages.Sum(i => i.Text.Length) + responseText.Length) / 4;

        // Get cost per token from model service
        var costPerToken = _modelService.GetCostPerToken(modelId);
        var costValue = (decimal)tokenUsage * costPerToken;

        // Simple parsing for score (mocking better extraction)
        double score = 85.0; // Default or parsed
        string summaryText = responseText;

        // Persist
        var sessionId = ResumeId.Of(Guid.NewGuid());
        var userId = UserId.Of(request.UserId);
        var config = new ResumeAnalysisConfiguration(
                       request.IncludeSkill,
                       request.IncludeExpireance,
                       request.IncludeEducation);

        var session = ResumeAnalysisSession.Create(sessionId, userId, modelId, config);

        var resultId = ResultId.Of(Guid.NewGuid());
        var resumeFileVo = ResumeFile.Of("UploadedResume", request.ResumeContent);
        var summaryVo = ResumeSummary.Of(summaryText);
        var scoreVo = CandidateScore.Of(score);
        var tokenCountVo = TokenCount.Of(tokenUsage);
        var costVo = CostEstimate.Of(costValue);

        var result = ResumeAnalysisResult.Create(
                    resultId, 
                    resumeFileVo, 
                    summaryVo, 
                    scoreVo, 
                    tokenCountVo, 
                    costVo);

        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AnalyzeResumeCommandResult(sessionId.Value, resultId.Value, summaryText, score, modelIdStr, providerName);
    }
}
