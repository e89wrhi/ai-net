using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using ImageGen.Data;
using ImageGen.Models;
using ImageGen.ValueObjects;
using Microsoft.Extensions.AI;

namespace ImageGen.Features.GenerateImage.V1;


internal class GenerateImageHandler : ICommandHandler<GenerateImageCommand, GenerateImageCommandResult>
{
    private readonly ImageGenDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IChatClient _chatClient;
    private readonly IAiModelService _modelService;

    public GenerateImageHandler(ImageGenDbContext dbContext, IAiModelService modelService, IAiOrchestrator orchestrator, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _modelService = modelService;
        _orchestrator = orchestrator;
        _chatClient = chatClient;
    }

    public async Task<GenerateImageCommandResult> Handle(GenerateImageCommand request, CancellationToken cancellationToken)
    {
        #region Prompt
        // Use ChatClient to "acknowledge" the generation or log intent
        var messages = new List<ChatMessage>
        {
            new ChatMessage(
                    role: ChatRole.System, 
                    content : "You are an image generation chatClient."),
            new ChatMessage(
                    role: ChatRole.User, 
                    content : $"Generate an image with prompt: {request.Prompt}. Style: {request.Style}, Size: {request.Size}")
        };
        #endregion

        Guard.Against.NullOrEmpty(request.Prompt, nameof(request.Prompt));

        // Use orchestrator to get the client based on requested model criteria
        var criteria = new ModelCriteria { ModelId = request.ModelId };
        var chatClient = await _orchestrator.GetClientAsync(criteria, cancellationToken);

        // Get actual model info from client metadata
        var clientMetadata = chatClient.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata;
        var modelIdStr = clientMetadata?.DefaultModelId ?? "default-model";
        var providerName = clientMetadata?.ProviderName ?? "Unknown";
        var modelId = ModelId.Of(modelIdStr);

        // Call AI Model
        var chatCompletion = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        var responseText = chatCompletion.Messages[0].Text ?? string.Empty;

        // Calculate Metadata & Usage
        var tokenUsage = chatCompletion.Usage?.TotalTokenCount ?? (messages.Sum(i => i.Text.Length) + responseText.Length) / 4;

        // Get cost per token from model service
        var costPerToken = _modelService.GetCostPerToken(modelId);
        var costValue = (decimal)tokenUsage * costPerToken;

        //  Image URL
        var imageUrl = $"https://generated-images.com/{Guid.NewGuid()}.png";

        // Persist
        var sessionId = ImageGenId.Of(Guid.NewGuid());
        var userId = UserId.Of(request.UserId);
        var config = new ImageGenerationConfiguration(
            Enums.ImageSize.Large,
            Enums.ImageStyle.Realistic,
            Enums.ImageQuality.Low,
            LanguageCode.Of("en"));
        var session = ImageGenerationSession.Create(sessionId, userId, modelId, config);

        var resultId = ImageGenResultId.Of(Guid.NewGuid());
        var promptVo = ImageGenerationPrompt.Of(request.Prompt);
        var imageVo = GeneratedImage.Of(imageUrl);
        var tokenCountVo = TokenCount.Of(tokenUsage); // Images usually cost "tokens" or flat rates
        var costVo = CostEstimate.Of(costValue);

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

        return new GenerateImageCommandResult(sessionId.Value, resultId.Value, imageUrl, modelIdStr, providerName);
    }
}
