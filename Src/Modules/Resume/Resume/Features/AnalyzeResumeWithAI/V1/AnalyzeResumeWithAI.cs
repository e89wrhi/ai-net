using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using Resume.Data;
using Resume.Models;
using Resume.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;
using Resume.Enums;

namespace Resume.Features.AnalyzeResumeWithAI.V1;

public record AnalyzeResumeWithAICommand(string ResumeContent) : ICommand<AnalyzeResumeWithAIResult>;

public record AnalyzeResumeWithAIResult(Guid SessionId, Guid ResultId, string Summary, double Score);

public class AnalyzeResumeWithAIEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/resume/analyze-ai",
                async (AnalyzeResumeWithAIRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new AnalyzeResumeWithAICommand(request.ResumeContent);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new AnalyzeResumeWithAIResponseDto(result.SessionId, result.ResultId, result.Summary, result.Score));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("AnalyzeResumeWithAI")
            .WithApiVersionSet(builder.NewApiVersionSet("Resume").Build())
            .Produces<AnalyzeResumeWithAIResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Analyze Resume with AI")
            .WithDescription("Uses AI to extract key information, skills, and experience from a resume.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public record AnalyzeResumeWithAIRequestDto(string ResumeContent);
public record AnalyzeResumeWithAIResponseDto(Guid SessionId, Guid ResultId, string Summary, double Score);

internal class AnalyzeResumeWithAIHandler : ICommandHandler<AnalyzeResumeWithAICommand, AnalyzeResumeWithAIResult>
{
    private readonly ResumeDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public AnalyzeResumeWithAIHandler(ResumeDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<AnalyzeResumeWithAIResult> Handle(AnalyzeResumeWithAICommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.ResumeContent, nameof(request.ResumeContent));

        var systemPrompt = "You are an HR expert system. Analyze the provided resume text. Extract a summary and provide a candidate score (0-100) based on professional standards.";
        
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, $"Resume Content:\n{request.ResumeContent}")
        };

        var completion = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);
        var responseText = completion.Message.Text ?? "Failed to analyze resume.";
        
        // Simple parsing for score (mocking better extraction)
        double score = 85.0; // Default or parsed
        string summaryText = responseText;

        // Persist
        var sessionId = ResumeId.Of(Guid.NewGuid());
        var userId = UserId.Of(Guid.NewGuid());
        var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "resume-model");
        var config = ResumeAnalysisConfiguration.Of(true, true);

        var session = ResumeAnalysisSession.Create(sessionId, userId, modelId, config);

        var resultId = ResultId.Of(Guid.NewGuid());
        var resumeFileVo = ResumeFile.Of("UploadedResume", request.ResumeContent);
        var summaryVo = ResumeSummary.Of(summaryText);
        var scoreVo = CandidateScore.Of(score);
        var tokenCountVo = TokenCount.Of(completion.Usage?.TotalTokenCount ?? 0);
        var costVo = CostEstimate.Of(0);

        var result = ResumeAnalysisResult.Create(resultId, resumeFileVo, summaryVo, scoreVo, tokenCountVo, costVo);
        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AnalyzeResumeWithAIResult(sessionId.Value, resultId.Value, summaryText, score);
    }
}
