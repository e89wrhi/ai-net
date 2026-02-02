using AI.Common.Core;
using Microsoft.Extensions.AI;
using User.Data;

namespace User.Features.GenerateUserPersonaWithAI.V1;


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
        Guard.Against.Default(request.UserId, nameof(request.UserId));

        var userId = UserId.Of(request.UserId);

        // Fetch module usage statistics to infer persona
        var moduleAnalytics = await _dbContext.ModuleAnalytics
            .ToListAsync(cancellationToken);

        var stats = string.Join(", ", moduleAnalytics.Select(m => $"{m.Module.Value}: {m.TotalRequests} visits"));

        var systemPrompt = "Based on a user's AI module usage statistics, generate a creative 'AI Persona' for them. Include a catchy persona name (e.g., 'The Digital Artisan'), a brief description, and a list of 3-5 traits. Output in JSON format with fields: personaName, description, traits.";

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, $"Usage Data: {stats}")
        };

        var completion = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);
        var responseJson = completion.Message.Text ?? "{\"personaName\": \"Power User\", \"description\": \"Dedicated AI explorer.\", \"traits\": \"Curious, Tech-savvy\"}";

        // Final parsing logic (mocking for breadth)
        string personaName = "The Multi-Modalist";
        string description = "You are a versatile user who leverages a wide range of AI capabilities across different domains.";
        string traits = "Versatile, Adaptive, Data-driven";

        try
        {
            if (responseJson.Contains("\"personaName\":")) personaName = responseJson.Split("\"personaName\":")[1].Split("\"")[1];
            if (responseJson.Contains("\"description\":")) description = responseJson.Split("\"description\":")[1].Split("\"")[1];
            if (responseJson.Contains("\"traits\":")) traits = responseJson.Split("\"traits\":")[1].Split("\"")[1];
        }
        catch { }

        return new GenerateUserPersonaWithAICommandResult(personaName, description, traits);
    }
}
