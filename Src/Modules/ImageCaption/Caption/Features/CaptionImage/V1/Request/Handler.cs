using AI.Common.Core;
using AiOrchestration.ValueObjects;
using ImageCaption.Data;
using ImageCaption.Models;
using ImageCaption.ValueObjects;
using Microsoft.Extensions.AI;

namespace ImageCaption.Features.CaptionImage.V1;


internal class AIImageCaptionHandler : ICommandHandler<AIImageCaptionCommand, AIImageCaptionCommandResult>
{
    private readonly ImageCaptionDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public AIImageCaptionHandler(ImageCaptionDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<AIImageCaptionCommandResult> Handle(AIImageCaptionCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.ImageUrlOrBase64, nameof(request.ImageUrlOrBase64));

        // Construct multi-modal message
        // Note: Microsoft.Extensions.AI supports multi-modal content via AIContent objects in ChatMessage

        List<AIContent> contents = new()
        {
            new TextContent("Describe this image in one concise sentence.")
        };

        if (request.ImageUrlOrBase64.StartsWith("http"))
        {
            contents.Add(new ImageContent(new Uri(request.ImageUrlOrBase64)));
        }
        else
        {
            // Assume base64
            var base64Data = request.ImageUrlOrBase64;
            if (base64Data.Contains(",")) base64Data = base64Data.Split(',')[1];
            contents.Add(new ImageContent(Convert.FromBase64String(base64Data), "image/jpeg"));
        }

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.User, contents)
        };

        var completion = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);
        var captionText = completion.Message.Text ?? "Unable to generate caption.";

        // Persist
        var sessionId = ImageCaptionId.Of(Guid.NewGuid());
        var userId = UserId.Of(Guid.NewGuid()); // Mock
        var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "vision-model");
        var config = ImageCaptionConfiguration.Of("standard");

        var session = ImageCaptionSession.Create(sessionId, userId, modelId, config);

        var resultId = ImageCaptionResultId.Of(Guid.NewGuid());
        var imageSource = ImageSource.Of(request.ImageUrlOrBase64.Length > 200 ? "base64-image" : request.ImageUrlOrBase64);
        var captionVo = CaptionText.Of(captionText);
        var confidenceVo = CaptionConfidence.Of(0.95);
        var tokenCountVo = TokenCount.Of(completion.Usage?.TotalTokenCount ?? 0);
        var costVo = CostEstimate.Of(0);

        var result = ImageCaptionResult.Create(resultId, imageSource, captionVo, confidenceVo, tokenCountVo, costVo);
        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AIImageCaptionResult(sessionId.Value, resultId.Value, captionText);
    }
}
