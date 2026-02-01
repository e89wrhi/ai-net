using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using ImageCaption.Data;
using ImageCaption.Models;
using ImageCaption.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace ImageCaption.Features.AIImageCaption.V1;

public record AIImageCaptionCommand(string ImageUrlOrBase64) : ICommand<AIImageCaptionResult>;

public record AIImageCaptionResult(Guid SessionId, Guid ResultId, string Caption);

public class AIImageCaptionEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/image/ai-caption",
                async (AIImageCaptionRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new AIImageCaptionCommand(request.ImageUrlOrBase64);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new AIImageCaptionResponseDto(result.SessionId, result.ResultId, result.Caption));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("AIImageCaption")
            .WithApiVersionSet(builder.NewApiVersionSet("Image").Build())
            .Produces<AIImageCaptionResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Generate Image Caption with AI")
            .WithDescription("Uses AI to generate a descriptive caption for the provided image.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public record AIImageCaptionRequestDto(string ImageUrlOrBase64);
public record AIImageCaptionResponseDto(Guid SessionId, Guid ResultId, string Caption);

internal class AIImageCaptionHandler : ICommandHandler<AIImageCaptionCommand, AIImageCaptionResult>
{
    private readonly ImageCaptionDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public AIImageCaptionHandler(ImageCaptionDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<AIImageCaptionResult> Handle(AIImageCaptionCommand request, CancellationToken cancellationToken)
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
