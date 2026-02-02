using AI.Common.Core;
using Microsoft.Extensions.AI;
using User.Data;

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
