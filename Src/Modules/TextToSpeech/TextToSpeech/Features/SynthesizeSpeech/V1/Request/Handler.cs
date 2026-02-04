using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using Microsoft.Extensions.AI;
using TextToSpeech.Data;
using TextToSpeech.Models;
using TextToSpeech.Services;
using TextToSpeech.ValueObjects;

namespace TextToSpeech.Features.SynthesizeSpeech.V1;


internal class SynthesizeSpeechHandler : ICommandHandler<SynthesizeSpeechCommand, SynthesizeSpeechCommandResult>
{
    private readonly TextToSpeechDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IChatClient _chatClient;
    private readonly IAiModelService _modelService;

    public SynthesizeSpeechHandler(TextToSpeechDbContext dbContext, IAiModelService modelService, IAiOrchestrator orchestrator, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
        _modelService = modelService;
        _orchestrator = orchestrator;
    }

    public async Task<SynthesizeSpeechCommandResult> Handle(SynthesizeSpeechCommand request, CancellationToken cancellationToken)
    {
        #region Prompt
        // Use IChatClient to orchestrate or log the synthesis intent
        var messages = new List<ChatMessage>
        {
            new ChatMessage(
                    role: ChatRole.System, 
                    content : "You are a speech synthesis chatClient."),
            new ChatMessage(
                    role: ChatRole.User, 
                    content : $"Synthesize this text using {request.Voice} voice at {request.Speed} speed in {request.Language}: {request.Text}")
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

        //  Audio URL (In real implementation, this would call a TTS service like OpenAI TTS or Azure Speech)
        var audioUrl = $"https://ai-audio-storage.com/{Guid.NewGuid()}.mp3";

        // Persist
        var sessionId = TextToSpeechId.Of(Guid.NewGuid());
        var userId = UserId.Of(request.UserId);
        var config = new TextToSpeechConfiguration(request.Voice, request.Speed, LanguageCode.Of(request.Language));

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

        return new SynthesizeSpeechCommandResult(sessionId.Value, resultId.Value, audioUrl, modelIdStr, providerName);
    }
}
