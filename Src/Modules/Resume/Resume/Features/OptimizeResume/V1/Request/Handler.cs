using AI.Common.Core;
using AiOrchestration.ValueObjects;
using Microsoft.Extensions.AI;
using Resume.Data;
using Resume.Models;
using Resume.ValueObjects;

namespace Resume.Features.OptimizeResume.V1;


internal class OptimizeResumeWithAIHandler : ICommandHandler<OptimizeResumeWithAICommand, OptimizeResumeWithAICommandResult>
{
    private readonly ResumeDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public OptimizeResumeWithAIHandler(ResumeDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<OptimizeResumeWithAICommandResult> Handle(OptimizeResumeWithAICommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.ResumeContent, nameof(request.ResumeContent));
        Guard.Against.NullOrEmpty(request.JobDescription, nameof(request.JobDescription));

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are a career coach. Optimize the provided resume to better match the given job description. Highlight key skills and rephrase experience where appropriate."),
            new ChatMessage(ChatRole.User, $"Resume:\n{request.ResumeContent}\n\nJob Description:\n{request.JobDescription}")
        };

        var completion = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);
        var optimizedText = completion.Message.Text ?? "Failed to optimize resume.";

        // Persist (using an existing session or creating a new one)
        var sessionId = ResumeId.Of(Guid.NewGuid());
        var userId = UserId.Of(Guid.NewGuid());
        var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "resume-optimization-model");
        var config = ResumeAnalysisConfiguration.Of(false, true);

        var session = ResumeAnalysisSession.Create(sessionId, userId, modelId, config);

        var resultId = ResultId.Of(Guid.NewGuid());
        var resumeFileVo = ResumeFile.Of("OptimizedResume", optimizedText);
        var summaryVo = ResumeSummary.Of("Optimization results");
        var scoreVo = CandidateScore.Of(90.0);
        var tokenCountVo = TokenCount.Of(completion.Usage?.TotalTokenCount ?? 0);
        var costVo = CostEstimate.Of(0);

        var result = ResumeAnalysisResult.Create(resultId, resumeFileVo, summaryVo, scoreVo, tokenCountVo, costVo);
        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new OptimizeResumeWithAIResult(resultId.Value, optimizedText);
    }
}
