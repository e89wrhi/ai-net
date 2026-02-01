using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using ImageEdit.Data;
using ImageEdit.Enums;
using ImageEdit.Models;
using ImageEdit.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace ImageEdit.Features.RemoveBackground.V1;

public record RemoveBackgroundCommand(string ImageUrlOrBase64) : ICommand<RemoveBackgroundResult>;

public record RemoveBackgroundResult(Guid SessionId, Guid ResultId, string ResultImageUrl);

public class RemoveBackgroundEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/imageedit/remove-background",
                async (RemoveBackgroundRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new RemoveBackgroundCommand(request.ImageUrlOrBase64);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new RemoveBackgroundResponseDto(result.SessionId, result.ResultId, result.ResultImageUrl));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("RemoveBackground")
            .WithApiVersionSet(builder.NewApiVersionSet("ImageEdit").Build())
            .Produces<RemoveBackgroundResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Remove Background with AI")
            .WithDescription("Uses AI to automatically detect and remove the background from an image.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public record RemoveBackgroundRequestDto(string ImageUrlOrBase64);
public record RemoveBackgroundResponseDto(Guid SessionId, Guid ResultId, string ResultImageUrl);

internal class RemoveBackgroundHandler : ICommandHandler<RemoveBackgroundCommand, RemoveBackgroundResult>
{
    private readonly ImageEditDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public RemoveBackgroundHandler(ImageEditDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<RemoveBackgroundResult> Handle(RemoveBackgroundCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.ImageUrlOrBase64, nameof(request.ImageUrlOrBase64));

        // Background removal usually uses a specialized model (like rembg or Segment Anything).
        // Here we simulate the process.
        
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.User, new List<AIContent>
            {
                new TextContent("Identify the subject of this image and prepare it for background removal."),
                request.ImageUrlOrBase64.StartsWith("http") 
                    ? new ImageContent(new Uri(request.ImageUrlOrBase64))
                    : new ImageContent(Convert.FromBase64String(request.ImageUrlOrBase64.Contains(",") ? request.ImageUrlOrBase64.Split(',')[1] : request.ImageUrlOrBase64), "image/jpeg")
            })
        };

        var completion = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);

        var resultImageUrl = request.ImageUrlOrBase64.StartsWith("http") 
            ? request.ImageUrlOrBase64 + "?bg_removed=true" 
            : "data:image/png;base64,BG_REMOVED_IMAGE_MOCK_DATA";

        // Persist
        var sessionId = ImageEditId.Of(Guid.NewGuid());
        var userId = UserId.Of(Guid.NewGuid()); 
        var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "vision-segment-model");
        var config = ImageEditConfiguration.Of("auto-segmentation");

        var session = ImageEditSession.Create(sessionId, userId, modelId, config);

        var resultId = ImageEditResultId.Of(Guid.NewGuid());
        var originalImage = ImageSource.Of(request.ImageUrlOrBase64.Length > 200 ? "base64-source" : request.ImageUrlOrBase64);
        var resultImage = EditedImage.Of(resultImageUrl);
        var promptVo = ImageEditPrompt.Of("Remove background automatically");
        var tokenCountVo = TokenCount.Of(completion.Usage?.TotalTokenCount ?? 50);
        var costVo = CostEstimate.Of(0.005m);

        var result = ImageEditResult.Create(resultId, originalImage, resultImage, promptVo, EditOperation.BackgroundRemoval, tokenCountVo, costVo);
        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new RemoveBackgroundResult(sessionId.Value, resultId.Value, resultImageUrl);
    }
}
