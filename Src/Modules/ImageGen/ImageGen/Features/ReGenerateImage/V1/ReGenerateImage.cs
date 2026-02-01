using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using ImageGen.Data;
using ImageGen.Models;
using ImageGen.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace ImageGen.Features.ReGenerateImage.V1;

public record ReGenerateImageCommand(Guid SessionId, string Instruction) : ICommand<ReGenerateImageResult>;

public record ReGenerateImageResult(Guid ResultId, string ImageUrl);

public class ReGenerateImageEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/imagegen/regenerate",
                async (ReGenerateImageRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new ReGenerateImageCommand(request.SessionId, request.Instruction);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new ReGenerateImageResponseDto(result.ResultId, result.ImageUrl));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("ReGenerateImage")
            .WithApiVersionSet(builder.NewApiVersionSet("ImageGen").Build())
            .Produces<ReGenerateImageResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Re-generate Image")
            .WithDescription("Re-generates or modifies an existing image based on new instructions.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public record ReGenerateImageRequestDto(Guid SessionId, string Instruction);
public record ReGenerateImageResponseDto(Guid ResultId, string ImageUrl);

internal class ReGenerateImageHandler : ICommandHandler<ReGenerateImageCommand, ReGenerateImageResult>
{
    private readonly ImageGenDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public ReGenerateImageHandler(ImageGenDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<ReGenerateImageResult> Handle(ReGenerateImageCommand request, CancellationToken cancellationToken)
    {
        // 1. Load Session
        var session = await _dbContext.Sessions.FindAsync(new object[] { CodeGenId.Of(request.SessionId) }, cancellationToken);
        if (session == null) throw new ImageGen.Exceptions.ImageGenNotFoundException(request.SessionId);

        // 2. Load context
        await _dbContext.Entry(session).Collection(s => s.Results).LoadAsync(cancellationToken);
        var lastResult = session.Results.OrderByDescending(r => r.GeneratedAt).FirstOrDefault();
        if (lastResult == null) throw new ImageGen.Exceptions.ImageGenResultNotFoundException(Guid.Empty);

        // 3. Orchestrate re-generation
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are an image generation refiner."),
            new ChatMessage(ChatRole.User, $"Original Prompt: {lastResult.Prompt.Value}"),
            new ChatMessage(ChatRole.User, $"Refinement Instruction: {request.Instruction}")
        };

        var completion = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);

        // Mock refined Image URL
        var imageUrl = $"https://generated-images.com/{Guid.NewGuid()}_refined.png";

        // 4. Update
        var resultId = ImageGenResultId.Of(Guid.NewGuid());
        var promptVo = ImageGenerationPrompt.Of(request.Instruction);
        var imageVo = GeneratedImage.Of(imageUrl);
        var tokenCountVo = TokenCount.Of(completion.Usage?.TotalTokenCount ?? 1200);
        var costVo = CostEstimate.Of(0.04m);

        var refinedResult = ImageGenerationResult.Create(
            resultId, 
            promptVo, 
            imageVo, 
            lastResult.Size, 
            lastResult.Style, 
            tokenCountVo, 
            costVo);

        session.AddResult(refinedResult);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ReGenerateImageResult(resultId.Value, imageUrl);
    }
}
