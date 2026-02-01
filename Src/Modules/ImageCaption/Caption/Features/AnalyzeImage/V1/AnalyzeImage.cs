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

namespace ImageCaption.Features.AnalyzeImage.V1;

public record AnalyzeImageCommand(string ImageUrlOrBase64) : ICommand<AnalyzeImageResult>;

public record AnalyzeImageResult(Guid SessionId, Guid ResultId, string Analysis);

public class AnalyzeImageEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/image/analyze",
                async (AnalyzeImageRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new AnalyzeImageCommand(request.ImageUrlOrBase64);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new AnalyzeImageResponseDto(result.SessionId, result.ResultId, result.Analysis));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("AnalyzeImage")
            .WithApiVersionSet(builder.NewApiVersionSet("Image").Build())
            .Produces<AnalyzeImageResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Analyze Image with AI")
            .WithDescription("Uses AI to provide a detailed analysis of the provided image, including objects, colors, and context.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public record AnalyzeImageRequestDto(string ImageUrlOrBase64);
public record AnalyzeImageResponseDto(Guid SessionId, Guid ResultId, string Analysis);

internal class AnalyzeImageHandler : ICommandHandler<AnalyzeImageCommand, AnalyzeImageResult>
{
    private readonly ImageCaptionDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public AnalyzeImageHandler(ImageCaptionDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<AnalyzeImageResult> Handle(AnalyzeImageCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.ImageUrlOrBase64, nameof(request.ImageUrlOrBase64));

        List<AIContent> contents = new()
        {
            new TextContent("Provide a detailed analysis of this image. Identify key objects, the setting, colors, and overall mood.")
        };

        if (request.ImageUrlOrBase64.StartsWith("http"))
        {
            contents.Add(new ImageContent(new Uri(request.ImageUrlOrBase64)));
        }
        else
        {
            var base64Data = request.ImageUrlOrBase64;
            if (base64Data.Contains(",")) base64Data = base64Data.Split(',')[1];
            contents.Add(new ImageContent(Convert.FromBase64String(base64Data), "image/jpeg"));
        }

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.User, contents)
        };

        var completion = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);
        var analysisText = completion.Message.Text ?? "Unable to analyze image.";

        // Persist
        var sessionId = ImageCaptionId.Of(Guid.NewGuid());
        var userId = UserId.Of(Guid.NewGuid());
        var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "vision-model");
        var config = ImageCaptionConfiguration.Of("detailed-analysis");

        var session = ImageCaptionSession.Create(sessionId, userId, modelId, config);

        var resultId = ImageCaptionResultId.Of(Guid.NewGuid());
        var imageSource = ImageSource.Of(request.ImageUrlOrBase64.Length > 200 ? "base64-image" : request.ImageUrlOrBase64);
        var captionVo = CaptionText.Of(analysisText); // We use CaptionText to store analysis for simplicity
        var confidenceVo = CaptionConfidence.Of(0.99);
        var tokenCountVo = TokenCount.Of(completion.Usage?.TotalTokenCount ?? 0);
        var costVo = CostEstimate.Of(0);

        var result = ImageCaptionResult.Create(resultId, imageSource, captionVo, confidenceVo, tokenCountVo, costVo);
        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AnalyzeImageResult(sessionId.Value, resultId.Value, analysisText);
    }
}
