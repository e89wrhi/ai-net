using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using User.Data;
using User.Models;
using User.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;
using Microsoft.EntityFrameworkCore;

namespace User.Features.AnalyzeUserUsageWithAI.V1;

public record AnalyzeUserUsageWithAICommand(Guid UserId) : ICommand<AnalyzeUserUsageWithAIResult>;

public record AnalyzeUserUsageWithAIResult(string UsageSummary, string Recommendations);

public class AnalyzeUserUsageWithAIEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/user/analyze-usage",
                async (AnalyzeUserUsageWithAIRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new AnalyzeUserUsageWithAICommand(request.UserId);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new AnalyzeUserUsageWithAIResponseDto(result.UsageSummary, result.Recommendations));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("AnalyzeUserUsageWithAI")
            .WithApiVersionSet(builder.NewApiVersionSet("User").Build())
            .Produces<AnalyzeUserUsageWithAIResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Analyze User AI Usage")
            .WithDescription("Uses AI to analyze a user's AI consumption patterns and provide personalized recommendations.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public record AnalyzeUserUsageWithAIRequestDto(Guid UserId);
public record AnalyzeUserUsageWithAIResponseDto(string UsageSummary, string Recommendations);

internal class AnalyzeUserUsageWithAIHandler : ICommandHandler<AnalyzeUserUsageWithAICommand, AnalyzeUserUsageWithAIResult>
{
    private readonly UserDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public AnalyzeUserUsageWithAIHandler(UserDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<AnalyzeUserUsageWithAIResult> Handle(AnalyzeUserUsageWithAICommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Default(request.UserId, nameof(request.UserId));

        var userId = UserId.Of(request.UserId);
        
        // Fetch user stats
        var userAnalytics = await _dbContext.UserAnalytics
            .FirstOrDefaultAsync(x => x.User == userId, cancellationToken);
            
        var moduleAnalytics = await _dbContext.ModuleAnalytics
            .ToListAsync(cancellationToken);

        var statsDescription = userAnalytics != null 
            ? $"Total Requests: {userAnalytics.TotalRequests}, Today: {userAnalytics.TodayRequests}, Week: {userAnalytics.WeekRequests}, Month: {userAnalytics.MonthRequests}."
            : "No global usage data available.";

        var moduleStats = string.Join("; ", moduleAnalytics.Select(m => $"{m.Module.Value}: {m.TotalRequests} total"));

        var systemPrompt = "You are an AI usage analyst. Analyze the following stats for a user and provide a professional summary of their behavior and personalized recommendations to improve their productivity with AI.";
        
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, $"User Stats: {statsDescription}\nModule Stats: {moduleStats}")
        };

        var completion = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);
        var responseText = completion.Message.Text ?? "Analysis failed.";
        
        // Split response into summary and recommendations (or mock split)
        var summary = responseText;
        var recommendations = "Try exploring new generative styles or automating repetitive tasks with our CodeGen module.";

        if (responseText.Contains("Recommendations:"))
        {
            var parts = responseText.Split("Recommendations:");
            summary = parts[0].Trim();
            recommendations = parts[1].Trim();
        }

        return new AnalyzeUserUsageWithAIResult(summary, recommendations);
    }
}
