using AI.Common.Core;
using AiOrchestration.ValueObjects;
using ImageGen.Data;
using ImageGen.Models;
using ImageGen.ValueObjects;
using Microsoft.Extensions.AI;
using Ardalis.GuardClauses;
using AiOrchestration.Services;

namespace ImageGen.Features.GenerateImage.V1;


internal class GenerateImageHandler : ICommandHandler<GenerateImageCommand, GenerateImageCommandResult>
{
    private readonly ImageGenDbContext _dbContext;
    private readonly IAiOrchestrator _chatClient;

    public GenerateImageHandler(ImageGenDbContext dbContext, IAiOrchestrator chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<GenerateImageCommandResult> Handle(GenerateImageCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Prompt, nameof(request.Prompt));

        // Use ChatClient to "acknowledge" the generation or log intent
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are an image generation chatClient."),
            new ChatMessage(ChatRole.User, $"Generate an image with prompt: {request.Prompt}. Style: {request.Style}, Size: {request.Size}")
        };

        // Use chatClient to get the best client
        var chatClient = await _chatClient.GetClientAsync(cancellationToken: cancellationToken);

        // This is a placeholder for real image generation call
        // In a real scenario, you'd use a dedicated IImageClient or similar
        var completion = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);

        // Mock Image URL
        var imageUrl = $"https://generated-images.com/{Guid.NewGuid()}.png";

        // Persist
        var sessionId = ImageGenId.Of(Guid.NewGuid());
        var userId = UserId.Of(Guid.NewGuid()); // Mock
        var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "dall-e-3");
        var config = ImageGenerationConfiguration.Of("standard");

        var session = ImageGenerationSession.Create(sessionId, userId, modelId, config);

        var resultId = ImageGenResultId.Of(Guid.NewGuid());
        var promptVo = ImageGenerationPrompt.Of(request.Prompt);
        var imageVo = GeneratedImage.Of(imageUrl);
        var tokenCountVo = TokenCount.Of(completion.Usage?.TotalTokenCount ?? 1000); // Images usually cost "tokens" or flat rates
        var costVo = CostEstimate.Of(0.04m);

        var result = ImageGenerationResult.Create(
            resultId,
            promptVo,
            imageVo,
            request.Size,
            request.Style,
            tokenCountVo,
            costVo);

        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new GenerateImageCommandResult(sessionId.Value, resultId.Value, imageUrl);
    }
}
