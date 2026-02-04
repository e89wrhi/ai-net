using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using ImageEdit.Data;
using ImageEdit.Enums;
using ImageEdit.Models;
using ImageEdit.ValueObjects;
using Microsoft.Extensions.AI;

namespace ImageEdit.Features.EnhanceImage.V1;


internal class AIEnhanceImageHandler : ICommandHandler<AIEnhanceImageCommand, AIEnhanceImageCommandResult>
{
    private readonly ImageEditDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IChatClient _chatClient;
    private readonly IAiModelService _modelService;

    public AIEnhanceImageHandler(ImageEditDbContext dbContext, IAiModelService modelService, IAiOrchestrator orchestrator, IChatClient chatClient)
    {
        _modelService = modelService;
        _orchestrator = orchestrator;
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<AIEnhanceImageCommandResult> Handle(AIEnhanceImageCommand request, CancellationToken cancellationToken)
    {
        #region Prompt
        var messages = new List<ChatMessage>
        {
            new ChatMessage(
                    role: ChatRole.User, 
                    content: new List<AIContent>
                        {
                            new TextContent($"User wants to enhance this image with the following prompt: {request.Prompt}. Analyze what needs to be changed."),
                            request.ImageUrlOrBase64.StartsWith("http")
                                ? new AIContent() { RawRepresentation = new Uri(request.ImageUrlOrBase64) }
                                : new AIContent() { RawRepresentation = Convert.FromBase64String(request.ImageUrlOrBase64.Contains(",") ? request.ImageUrlOrBase64 : request.ImageUrlOrBase64) }
                        })
        };
        #endregion

        Guard.Against.NullOrEmpty(request.ImageUrlOrBase64, nameof(request.ImageUrlOrBase64));

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

        // In a real application, we would send this to an Image Editing model (like DALL-E Edits or ControlNet).
        // Here, we use IChatClient to simulate "deciding" on the edit, then returning a mock result.

        //  result: Usually the model would return an image URL or bytes.
        // We'll return a placeholder that represents the "edited" version.
        var resultImageUrl = request.ImageUrlOrBase64.StartsWith("http")
            ? request.ImageUrlOrBase64 + "?enhanced=true"
            : "data:image/jpeg;base64,ENHANCED_IMAGE_MOCK_DATA";

        // Persist
        var sessionId = ImageEditId.Of(Guid.NewGuid());
        var userId = UserId.Of(request.UserId);
        var config = new ImageEditConfiguration(
            request.Quality, request.Format);

        var session = ImageEditSession.Create(sessionId, userId, modelId, config);

        var resultId = ImageEditResultId.Of(Guid.NewGuid());
        var originalImage = ImageSource.Of(request.ImageUrlOrBase64.Length > 200 ? "base64-source" : request.ImageUrlOrBase64);
        var resultImage = EditedImage.Of(resultImageUrl);
        var promptVo = ImageEditPrompt.Of(request.Prompt ?? "Enhance details");
        var tokenCountVo = TokenCount.Of(tokenUsage);
        var costVo = CostEstimate.Of(costValue);

        var result = ImageEditResult.Create(
                    resultId, 
                    originalImage, 
                    resultImage, 
                    promptVo, 
                    EditOperation.Enhance, 
                    tokenCountVo, 
                    costVo);
        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AIEnhanceImageCommandResult(sessionId.Value, resultId.Value, resultImageUrl, modelIdStr, providerName);
    }
}
