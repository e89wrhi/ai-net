using AI.Common.Core;
using Microsoft.Extensions.AI;
using Microsoft.EntityFrameworkCore;
using User.Data;
using Ardalis.GuardClauses;
using User.ValueObjects;

namespace User.Features.AnalyzeUserUsage.V1;


internal class AnalyzeUserUsageWithAIHandler : ICommandHandler<AnalyzeUserUsageWithAICommand, AnalyzeUserUsageWithAICommandResult>
{
    private readonly UserDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public AnalyzeUserUsageWithAIHandler(UserDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<AnalyzeUserUsageWithAICommandResult> Handle(AnalyzeUserUsageWithAICommand request, CancellationToken cancellationToken)
    {
        Ardalis.GuardClauses.Guard.Against.Default(request.UserId, nameof(request.UserId));

        var userId = UserId.Of(request.UserId);

        // Fetch user stats
        var userAnalytics = await _dbContext.UserAnalytics
            .FirstOrDefaultAsync(x => x.User == userId, cancellationToken);

        var moduleAnalytics = await _dbContext.ModuleAnalytics
            .ToListAsync(cancellationToken);

        var statsDescription = userAnalytics != null
            ? $"Total Requests: {userAnalytics.TotalRequests}, Today: {userAnalytics.TodayRequests}, Week: {userAnalytics.WeekRequests}, Month: {userAnalytics.MonthRequests}."
            : "No global usage data available.";

        var moduleStats = string.Join("; ", moduleAnalytics.Select(m => $"{m.Module}: {m.TotalRequests} total"));

        var systemPrompt = "You are an AI usage analyst. Analyze the following stats for a user and provide a professional summary of their behavior and personalized recommendations to improve their productivity with AI. Please use the following format:\nSummary: [The summary]\nRecommendations: [The recommendations]";

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, $"User Stats: {statsDescription}\nModule Stats: {moduleStats}")
        };

        var completion = await _chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        var responseText = completion.Messages[0].Text ?? "Analysis failed.";

        // Split response into summary and recommendations
        var summary = responseText;
        var recommendations = "Continue exploring the platform to see more personalized recommendations.";

        if (responseText.Contains("Recommendations:", StringComparison.OrdinalIgnoreCase))
        {
            var parts = responseText.Split(new[] { "Recommendations:", "recommendations:" }, StringSplitOptions.None);
            summary = parts[0].Replace("Summary:", "", StringComparison.OrdinalIgnoreCase).Trim();
            recommendations = parts[1].Trim();
        }
        else if (responseText.Contains("Summary:", StringComparison.OrdinalIgnoreCase))
        {
            summary = responseText.Replace("Summary:", "", StringComparison.OrdinalIgnoreCase).Trim();
        }

        return new AnalyzeUserUsageWithAICommandResult(summary, recommendations);
    }
}
