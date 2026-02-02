using AI.Common.Core;
using AiOrchestration.ValueObjects;
using ImageEdit.Data;
using ImageEdit.Enums;
using ImageEdit.Models;
using ImageEdit.ValueObjects;
using Microsoft.Extensions.AI;

namespace ImageEdit.Features.AiEnhanceImage.V1;


internal class AIEnhanceImageHandler : ICommandHandler<AIEnhanceImageCommand, AIEnhanceImageCommandResult>
{
    private readonly ImageEditDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public AIEnhanceImageHandler(ImageEditDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<AIEnhanceImageCommandResult> Handle(AIEnhanceImageCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.ImageUrlOrBase64, nameof(request.ImageUrlOrBase64));

        // In a real application, we would send this to an Image Editing model (like DALL-E Edits or ControlNet).
        // Here, we use IChatClient to simulate "deciding" on the edit, then returning a mock result.

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.User, new List<AIContent>
            {
                new TextContent($"User wants to enhance this image with the following prompt: {request.Prompt}. Analyze what needs to be changed."),
                request.ImageUrlOrBase64.StartsWith("http")
                    ? new ImageContent(new Uri(request.ImageUrlOrBase64))
                    : new ImageContent(Convert.FromBase64String(request.ImageUrlOrBase64.Contains(",") ? request.ImageUrlOrBase64.Split(',')[1] : request.ImageUrlOrBase64), "image/jpeg")
            })
        };

        var completion = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);

        // Mock result: Usually the model would return an image URL or bytes.
        // We'll return a placeholder that represents the "edited" version.
        var resultImageUrl = request.ImageUrlOrBase64.StartsWith("http")
            ? request.ImageUrlOrBase64 + "?enhanced=true"
            : "data:image/jpeg;base64,ENHANCED_IMAGE_MOCK_DATA";

        // Persist
        var sessionId = ImageEditId.Of(Guid.NewGuid());
        var userId = UserId.Of(Guid.NewGuid()); // Mock
        var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "edit-model");
        var config = ImageEditConfiguration.Of("high-quality");

        var session = ImageEditSession.Create(sessionId, userId, modelId, config);

        var resultId = ImageEditResultId.Of(Guid.NewGuid());
        var originalImage = ImageSource.Of(request.ImageUrlOrBase64.Length > 200 ? "base64-source" : request.ImageUrlOrBase64);
        var resultImage = EditedImage.Of(resultImageUrl);
        var promptVo = ImageEditPrompt.Of(request.Prompt ?? "Enhance details");
        var tokenCountVo = TokenCount.Of(completion.Usage?.TotalTokenCount ?? 100);
        var costVo = CostEstimate.Of(0.01m);

        var result = ImageEditResult.Create(resultId, originalImage, resultImage, promptVo, EditOperation.Enhance, tokenCountVo, costVo);
        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AIEnhanceImageCommandResult(sessionId.Value, resultId.Value, resultImageUrl);
    }
}
