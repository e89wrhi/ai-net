using AI.Common.Core;
using Microsoft.Extensions.AI;
using Microsoft.EntityFrameworkCore;
using User.Data;
using System.Text.Json;
using Ardalis.GuardClauses;

namespace User.Features.GenerateUserPersona.V1;


internal class GenerateUserPersonaWithAIHandler : ICommandHandler<GenerateUserPersonaWithAICommand, GenerateUserPersonaWithAICommandResult>
{
    private readonly UserDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public GenerateUserPersonaWithAIHandler(UserDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<GenerateUserPersonaWithAICommandResult> Handle(GenerateUserPersonaWithAICommand request, CancellationToken cancellationToken)
    {
        Ardalis.GuardClauses.Guard.Against.Default(request.UserId, nameof(request.UserId));

        var userId = UserId.Of(request.UserId);

        // Fetch user global usage statistics
        var userAnalytics = await _dbContext.UserAnalytics
            .FirstOrDefaultAsync(x => x.User == userId, cancellationToken);
        
        // Fetch module usage statistics (Note: currently global, as per-user module stats are not yet implemented)
        var moduleAnalytics = await _dbContext.ModuleAnalytics
            .ToListAsync(cancellationToken);

        var stats = string.Join(", ", moduleAnalytics.Select(m => $"{m.Module}: {m.TotalRequests} visits"));
        var userSummary = userAnalytics != null 
            ? $"User has made {userAnalytics.TotalRequests} total requests." 
            : "No usage data found for user.";

        var systemPrompt = "Based on a user's AI module usage statistics, generate a creative 'AI Persona' for them. Include a catchy persona name (e.g., 'The Digital Artisan'), a brief description, and a list of 3-5 traits. Output in JSON format with fields: personaName, description, traits.";

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, $"Usage Data: {stats}\nUser Summary: {userSummary}")
        };

        var completion = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);
        var responseJson = completion.Message.Text ?? string.Empty;

        string personaName = "The Enthusiastic User";
        string description = "A dedicated explorer of AI capabilities across the platform.";
        string traits = "Curious, Engaged, Growing";

        try
        {
            if (!string.IsNullOrWhiteSpace(responseJson))
            {
                // Simple cleanup of AI response if it contains markdown formatting
                if (responseJson.Contains("```json"))
                {
                    responseJson = responseJson.Split("```json")[1].Split("```")[0];
                }
                else if (responseJson.Contains("```"))
                {
                    responseJson = responseJson.Split("```")[1].Split("```")[0];
                }

                using var doc = JsonDocument.Parse(responseJson);
                var root = doc.RootElement;
                if (root.TryGetProperty("personaName", out var p)) personaName = p.GetString() ?? personaName;
                if (root.TryGetProperty("description", out var d)) description = d.GetString() ?? description;
                if (root.TryGetProperty("traits", out var t))
                {
                    if (t.ValueKind == JsonValueKind.Array)
                        traits = string.Join(", ", t.EnumerateArray().Select(x => x.GetString()));
                    else
                        traits = t.GetString() ?? traits;
                }
            }
        }
        catch 
        { 
            // Fallback to default if parsing fails
        }

        return new GenerateUserPersonaWithAICommandResult(personaName, description, traits);
    }
}
