using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using ImageGen.Data;
using ImageGen.Enums;
using ImageGen.Models;
using ImageGen.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace ImageGen.Features.GenerateImage.V1;

public record GenerateImageCommand(string Prompt, ImageSize Size, ImageStyle Style) : ICommand<GenerateImageResult>;

public record GenerateImageResult(Guid SessionId, Guid ResultId, string ImageUrl);

public class GenerateImageEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/imagegen/generate",
                async (GenerateImageRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new GenerateImageCommand(request.Prompt, request.Size, request.Style);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new GenerateImageResponseDto(result.SessionId, result.ResultId, result.ImageUrl));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GenerateImage")
            .WithApiVersionSet(builder.NewApiVersionSet("ImageGen").Build())
            .Produces<GenerateImageResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Generate Image")
            .WithDescription("Generates an image from a text prompt using AI.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public record GenerateImageRequestDto(string Prompt, ImageSize Size, ImageStyle Style);
public record GenerateImageResponseDto(Guid SessionId, Guid ResultId, string ImageUrl);

internal class GenerateImageHandler : ICommandHandler<GenerateImageCommand, GenerateImageResult>
{
    private readonly ImageGenDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public GenerateImageHandler(ImageGenDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<GenerateImageResult> Handle(GenerateImageCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Prompt, nameof(request.Prompt));

        // Use ChatClient to "acknowledge" the generation or log intent
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are an image generation orchestrator."),
            new ChatMessage(ChatRole.User, $"Generate an image with prompt: {request.Prompt}. Style: {request.Style}, Size: {request.Size}")
        };

        // This is a placeholder for real image generation call
        // In a real scenario, you'd use a dedicated IImageClient or similar
        var completion = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);

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

        return new GenerateImageResult(sessionId.Value, resultId.Value, imageUrl);
    }
}
