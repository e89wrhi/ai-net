using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using ImageCaption.Data;
using ImageCaption.Models;
using ImageCaption.ValueObjects;
using Microsoft.Extensions.AI;

namespace ImageCaption.Features.CaptionImage.V1;


internal class AIImageCaptionHandler : ICommandHandler<ImageCaptionCommand, ImageCaptionCommandResult>
{
    private readonly ImageCaptionDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IChatClient _chatClient;
    private readonly IAiModelService _modelService;

    public AIImageCaptionHandler(ImageCaptionDbContext dbContext, IAiModelService modelService, IAiOrchestrator orchestrator, IChatClient chatClient)
    {
        _modelService = modelService;
        _orchestrator = orchestrator;
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<ImageCaptionCommandResult> Handle(ImageCaptionCommand request, CancellationToken cancellationToken)
    {
        #region Prompt
        List<AIContent> contents = new()
        {
            new TextContent("Describe this image in one concise sentence.")
        };

        var messages = new List<ChatMessage>
        {
             new ChatMessage(
                    role: ChatRole.System,
                    content: ""),
             new ChatMessage(
                    role: ChatRole.User,
                    content: contents)
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
        var responseText = chatCompletion.Messages[0].Text ?? "Unable to generate caption.";

        // Calculate Metadata & Usage
        var tokenUsage = chatCompletion.Usage?.TotalTokenCount ?? (messages.Sum(i => i.Text.Length) + responseText.Length) / 4;

        // Get cost per token from model service
        var costPerToken = _modelService.GetCostPerToken(modelId);
        var costValue = (decimal)tokenUsage * costPerToken;

        // Construct multi-modal message
        // Note: Microsoft.Extensions.AI supports multi-modal content via AIContent objects in ChatMessage

        if (request.ImageUrlOrBase64.StartsWith("http"))
        {
            contents.Add(new AIContent() { RawRepresentation = new Uri(request.ImageUrlOrBase64) });
        }
        else
        {
            // Assume base64
            var base64Data = request.ImageUrlOrBase64;
            if (base64Data.Contains(",")) base64Data = base64Data.Split(',')[1];
            contents.Add(new AIContent() { RawRepresentation = Convert.FromBase64String(base64Data) });
        }

        // Persist
        var sessionId = ImageCaptionId.Of(Guid.NewGuid());
        var userId = UserId.Of(request.UserId); 
        var config = new ImageCaptionConfiguration( Enums.CaptionDetailLevel.Detailed, 
            LanguageCode.Of("en"));

        var session = ImageCaptionSession.Create(sessionId, userId, modelId, config);

        var resultId = ImageCaptionResultId.Of(Guid.NewGuid());
        var imageSource = ImageSource.Of(request.ImageUrlOrBase64.Length > 200 ? "base64-image" : request.ImageUrlOrBase64);
        var captionVo = CaptionText.Of(responseText);
        var confidenceVo = CaptionConfidence.Of(0.95);
        var tokenCountVo = TokenCount.Of(tokenUsage);
        var costVo = CostEstimate.Of(costValue);

        var result = ImageCaptionResult.Create(
                resultId, 
                imageSource, 
                captionVo, 
                confidenceVo, 
                tokenCountVo, 
                costVo);

        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ImageCaptionCommandResult(sessionId.Value, resultId.Value, responseText, modelIdStr, providerName);
    }
}
