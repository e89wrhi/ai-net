using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using Microsoft.Extensions.AI;
using Resume.Data;
using Resume.Models;
using Resume.ValueObjects;

namespace Resume.Features.OptimizeResume.V1;


internal class OptimizeResumeWithAIHandler : ICommandHandler<OptimizeResumeCommand, OptimizeResumeCommandResult>
{
    private readonly ResumeDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IChatClient _chatClient;
    private readonly IAiModelService _modelService;

    public OptimizeResumeWithAIHandler(ResumeDbContext dbContext, IAiModelService modelService, IAiOrchestrator orchestrator, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _modelService = modelService;
        _orchestrator = orchestrator;
        _chatClient = chatClient;
    }

    public async Task<OptimizeResumeCommandResult> Handle(OptimizeResumeCommand request, CancellationToken cancellationToken)
    {
        #region Prompt
        var messages = new List<ChatMessage>
        {
            new ChatMessage(
                    role: ChatRole.System, 
                    content: "You are a career coach. Optimize the provided resume to better match the given job description. Highlight key skills and rephrase experience where appropriate."),
            new ChatMessage(
                    role: ChatRole.User, 
                    content : $"Resume:\n{request.ResumeContent}\n\nJob Description:\n{request.JobDescription}")
        };
        #endregion

        Guard.Against.NullOrEmpty(request.ResumeContent, nameof(request.ResumeContent));
        Guard.Against.NullOrEmpty(request.JobDescription, nameof(request.JobDescription));

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
        var responseText = chatCompletion.Messages[0].Text ?? "Failed to optimize resume.";

        // Calculate Metadata & Usage
        var tokenUsage = chatCompletion.Usage?.TotalTokenCount ?? (messages.Sum(i => i.Text.Length) + responseText.Length) / 4;

        // Get cost per token from model service
        var costPerToken = _modelService.GetCostPerToken(modelId);
        var costValue = (decimal)tokenUsage * costPerToken;

        // Persist (using an existing session or creating a new one)
        var sessionId = ResumeId.Of(Guid.NewGuid());
        var userId = UserId.Of(request.UserId);
        var config = new ResumeAnalysisConfiguration(
                        request.IncludeSkill,
                        request.IncludeExpireance,
                        request.IncludeEducation);

        var session = ResumeAnalysisSession.Create(sessionId, userId, modelId, config);

        var resultId = ResultId.Of(Guid.NewGuid());
        var resumeFileVo = ResumeFile.Of("OptimizedResume", responseText);
        var summaryVo = ResumeSummary.Of("Optimization results");
        var scoreVo = CandidateScore.Of(90.0);
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

        return new OptimizeResumeCommandResult(resultId.Value, responseText, modelIdStr, providerName);
    }
}
