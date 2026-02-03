using AI.Common.Core;
using AiOrchestration.ValueObjects;
using ImageGen.Data;
using ImageGen.Models;
using ImageGen.ValueObjects;
using Ardalis.GuardClauses;
using AiOrchestration.Services;
using Microsoft.Extensions.AI;

namespace ImageGen.Features.ReGenerateImage.V1;

internal class ReGenerateImageHandler : ICommandHandler<ReGenerateImageCommand, ReGenerateImageCommandResult>
{
    private readonly ImageGenDbContext _dbContext;
    private readonly IAiOrchestrator _chatClient;

    public ReGenerateImageHandler(ImageGenDbContext dbContext, IAiOrchestrator chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<ReGenerateImageCommandResult> Handle(ReGenerateImageCommand request, CancellationToken cancellationToken)
    {
        // Load Session
        var session = await _dbContext.Sessions.FindAsync(new object[] { CodeGenId.Of(request.SessionId) }, cancellationToken);
        if (session == null) throw new ImageGen.Exceptions.ImageGenNotFoundException(request.SessionId);

        // Load context
        await _dbContext.Entry(session).Collection(s => s.Results).LoadAsync(cancellationToken);
        var lastResult = session.Results.OrderByDescending(r => r.GeneratedAt).FirstOrDefault();
        if (lastResult == null) throw new ImageGen.Exceptions.ImageGenResultNotFoundException(Guid.Empty);

        // Orchestrate re-generation
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are an image generation refiner."),
            new ChatMessage(ChatRole.User, $"Original Prompt: {lastResult.Prompt.Value}"),
            new ChatMessage(ChatRole.User, $"Refinement Instruction: {request.Instruction}")
        };

        // Use chatClient to get the best client
        var chatClient = await _chatClient.GetClientAsync(cancellationToken: cancellationToken);
        var completion = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);

        // Mock refined Image URL
        var imageUrl = $"https://generated-images.com/{Guid.NewGuid()}_refined.png";

        // Update
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

        return new ReGenerateImageCommandResult(resultId.Value, imageUrl);
    }
}

