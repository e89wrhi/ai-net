using AI.Common.Core;
using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Payment.Data;
using Payment.ValueObjects;

namespace Payment.Features.ForecastSpending.V1;


internal class ForecastSpendingWithAIHandler : ICommandHandler<ForecastSpendingWithAICommand, ForecastSpendingWithAICommandResult>
{
    private readonly PaymentDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public ForecastSpendingWithAIHandler(PaymentDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<ForecastSpendingWithAICommandResult> Handle(ForecastSpendingWithAICommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Default(request.UserId, nameof(request.UserId));

        var userId = UserId.Of(request.UserId);

        // Fetch recent usage charges
        var charges = await _dbContext.UsageCharges
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Take(50)
            .ToListAsync(cancellationToken);

        if (!charges.Any())
        {
            return new ForecastSpendingWithAICommandResult(0, "USD", "No usage data found to generate a forecast.");
        }

        var chargesData = string.Join(", ", charges.Select(c => $"Date: {c.CreatedAt:yyyy-MM-dd}, Cost: {c.Cost.Amount} {c.Cost.Currency}, Module: {c.Module}"));

        var systemPrompt = "You are a financial analyst specializing in AI cloud costs. Analyze the provided historical usage data and forecast the next month's spending. Provide the forecasted numeric value and a brief insight in JSON format with fields: forecastedAmount, insights.";

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, $"Historical Charges: {chargesData}")
        };

        var completion = await _chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        var responseJson = completion.Messages[0].Text ?? "{\"forecastedAmount\": 0, \"insights\": \"Forecast failed.\"}";

        decimal forecastedAmount = 0;
        string insights = "Based on your current trend, your spending is expected to remain stable.";

        try
        {
            if (responseJson.Contains("\"forecastedAmount\":"))
            {
                var val = responseJson.Split("\"forecastedAmount\":")[1].Split(",")[0].Split("}")[0].Trim();
                decimal.TryParse(val, out forecastedAmount);
            }
            if (responseJson.Contains("\"insights\":")) insights = responseJson.Split("\"insights\":")[1].Split("\"")[1];
        }
        catch { }

        return new ForecastSpendingWithAICommandResult(forecastedAmount, charges.First().Cost.Currency, insights);
    }
}
