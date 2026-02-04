using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using ImageGen.Data;
using ImageGen.Exceptions;
using ImageGen.Models;
using ImageGen.ValueObjects;
using Microsoft.Extensions.AI;

namespace ImageGen.Features.ReGenerateImage.V1;

internal class ReGenerateImageHandler : ICommandHandler<ReGenerateImageCommand, ReGenerateImageCommandResult>
{
    private readonly ImageGenDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IChatClient _chatClient;
    private readonly IAiModelService _modelService;

    public ReGenerateImageHandler(ImageGenDbContext dbContext, IAiModelService modelService, IAiOrchestrator orchestrator, IChatClient chatClient)
    {
        _modelService = modelService;
        _orchestrator = orchestrator;
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<ReGenerateImageCommandResult> Handle(ReGenerateImageCommand request, CancellationToken cancellationToken)
    {
        // Use orchestrator to get the client based on requested model criteria
        var criteria = new ModelCriteria { ModelId = request.ModelId };
        var chatClient = await _orchestrator.GetClientAsync(criteria, cancellationToken);

        // Get actual model info from client metadata
        var clientMetadata = chatClient.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata;
        var modelIdStr = clientMetadata?.DefaultModelId ?? "default-model";
        var providerName = clientMetadata?.ProviderName ?? "Unknown";
        var modelId = ModelId.Of(modelIdStr);

        // Load Session
        var session = await _dbContext.Sessions.FindAsync(new object[] { ImageGenId.Of(request.SessionId) }, cancellationToken);
        if (session == null) throw new ImageGenNotFoundException(request.SessionId);

        // Load context
        await _dbContext.Entry(session).Collection(s => s.Results).LoadAsync(cancellationToken);
        var lastResult = session.Results.OrderByDescending(r => r.GeneratedAt).FirstOrDefault();
        if (lastResult == null) throw new ImageGenResultNotFoundException(Guid.Empty);

        #region Prompt
        // Orchestrate re-generation
        var messages = new List<ChatMessage>
        {
            new ChatMessage(
                    role: ChatRole.System, 
                    content: "You are an image generation refiner."),
            new ChatMessage(
                    role: ChatRole.User, 
                    content : $"Original Prompt: {lastResult.Prompt.Value}"),
            new ChatMessage(
                    role: ChatRole.User, 
                    content : $"Refinement Instruction: {request.Instruction}")
        };
        #endregion

        // Call AI Model
        var chatCompletion = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        var responseText = chatCompletion.Messages[0].Text ?? string.Empty;

        // Calculate Metadata & Usage
        var tokenUsage = chatCompletion.Usage?.TotalTokenCount ?? (messages.Sum(i => i.Text.Length) + responseText.Length) / 4;

        // Get cost per token from model service
        var costPerToken = _modelService.GetCostPerToken(modelId);
        var costValue = (decimal)tokenUsage * costPerToken;

        //  refined Image URL
        var imageUrl = $"https://generated-images.com/{Guid.NewGuid()}_refined.png";

        // Update
        var resultId = ImageGenResultId.Of(Guid.NewGuid());
        var promptVo = ImageGenerationPrompt.Of(request.Instruction);
        var imageVo = GeneratedImage.Of(imageUrl);
        var tokenCountVo = TokenCount.Of(tokenUsage);
        var costVo = CostEstimate.Of(costValue);

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

        return new ReGenerateImageCommandResult(resultId.Value, imageUrl, modelIdStr, providerName);
    }
}

