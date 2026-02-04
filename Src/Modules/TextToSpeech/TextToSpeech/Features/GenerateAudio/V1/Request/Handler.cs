using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using Microsoft.Extensions.AI;
using TextToSpeech.Data;
using TextToSpeech.Enums;
using TextToSpeech.Models;
using TextToSpeech.ValueObjects;

namespace TextToSpeech.Features.GenerateAudio.V1;


internal class GenerateAudioWithAIHandler : ICommandHandler<GenerateAudioCommand, GenerateAudioCommandResult>
{
    private readonly TextToSpeechDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IChatClient _chatClient;
    private readonly IAiModelService _modelService;

    public GenerateAudioWithAIHandler(TextToSpeechDbContext dbContext, IAiModelService modelService, IAiOrchestrator orchestrator, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
        _modelService = modelService;
        _orchestrator = orchestrator;
    }

    public async Task<GenerateAudioCommandResult> Handle(GenerateAudioCommand request, CancellationToken cancellationToken)
    {
        #region Prompt
        var messages = new List<ChatMessage>
        {
             new ChatMessage(
                    role: ChatRole.System,
                    content: ""),
             new ChatMessage(
                    role: ChatRole.User,
                    content: "")
        };
        #endregion

        Guard.Against.NullOrEmpty(request.Text, nameof(request.Text));

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
        var responseText = chatCompletion.Messages[0].Text ?? string.Empty;

        // Calculate Metadata & Usage
        var tokenUsage = chatCompletion.Usage?.TotalTokenCount ?? (messages.Sum(i => i.Text.Length) + responseText.Length) / 4;

        // Get cost per token from model service
        var costPerToken = _modelService.GetCostPerToken(modelId);
        var costValue = (decimal)tokenUsage * costPerToken;

        //  Logic for AI Voice Generation
        var audioUrl = $"https://cdn.ai-voices.com/gen/{Guid.NewGuid()}.wav";

        // Persist
        var sessionId = TextToSpeechId.Of(Guid.NewGuid());
        var userId = UserId.Of(request.UserId);
        var config = new TextToSpeechConfiguration(request.Voice, SpeechSpeed.Normal, LanguageCode.Of("en"));

        var session = TextToSpeechSession.Create(sessionId, userId, modelId, config);

        var resultId = TextToSpeechResultId.Of(Guid.NewGuid());
        var speechVo = SynthesizedSpeech.Of(audioUrl);
        var tokenCountVo = TokenCount.Of(tokenUsage);
        var costVo = CostEstimate.Of(costValue);

        var result = TextToSpeechResult.Create(
                resultId, 
                request.Text, 
                speechVo, 
                tokenCountVo, 
                costVo);

        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new GenerateAudioCommandResult(sessionId.Value, audioUrl, modelIdStr, providerName);
    }
}
