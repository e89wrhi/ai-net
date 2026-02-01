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

namespace Resume.Features.OptimizeResumeWithAI.V1;

public record OptimizeResumeWithAICommand(string ResumeContent, string JobDescription) : ICommand<OptimizeResumeWithAIResult>;

public record OptimizeResumeWithAIResult(Guid ResultId, string OptimizedResume);

public class OptimizeResumeWithAIEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/resume/optimize",
                async (OptimizeResumeWithAIRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new OptimizeResumeWithAICommand(request.ResumeContent, request.JobDescription);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new OptimizeResumeWithAIResponseDto(result.ResultId, result.OptimizedResume));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("OptimizeResumeWithAI")
            .WithApiVersionSet(builder.NewApiVersionSet("Resume").Build())
            .Produces<OptimizeResumeWithAIResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Optimize Resume for Job Description")
            .WithDescription("Uses AI to suggest improvements and tailor the resume for a specific job description.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public record OptimizeResumeWithAIRequestDto(string ResumeContent, string JobDescription);
public record OptimizeResumeWithAIResponseDto(Guid ResultId, string OptimizedResume);

internal class OptimizeResumeWithAIHandler : ICommandHandler<OptimizeResumeWithAICommand, OptimizeResumeWithAIResult>
{
    private readonly ResumeDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public OptimizeResumeWithAIHandler(ResumeDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<OptimizeResumeWithAIResult> Handle(OptimizeResumeWithAICommand request, CancellationToken cancellationToken)
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
