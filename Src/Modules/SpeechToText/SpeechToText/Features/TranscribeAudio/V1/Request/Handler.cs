using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using Microsoft.Extensions.AI;
using SpeechToText.Data;
using SpeechToText.Models;
using SpeechToText.ValueObjects;

namespace SpeechToText.Features.TranscribeAudio.V1;


internal class TranscribeAudioHandler : ICommandHandler<TranscribeAudioCommand, TranscribeAudioCommandResult>
{
    private readonly SpeechToTextDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IChatClient _chatClient;
    private readonly IAiModelService _modelService;

    public TranscribeAudioHandler(SpeechToTextDbContext dbContext, IAiModelService modelService, IAiOrchestrator orchestrator, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
        _modelService = modelService;
        _orchestrator = orchestrator;
    }

    public async Task<TranscribeAudioCommandResult> Handle(TranscribeAudioCommand request, CancellationToken cancellationToken)
    {
        #region Prompt
        // Multi-modal models can process audio if supported, 
        // otherwise this serves as an orchestration point.
        var messages = new List<ChatMessage>
        {
            new ChatMessage(
                    role: ChatRole.System, 
                    content : "You are a transcription engine."),
            new ChatMessage(
                    role: ChatRole.User, 
                    content : $"Please transcribe the audio at this URL: {request.AudioUrl} in language: {request.Language}")
        };
        #endregion

        Guard.Against.NullOrEmpty(request.AudioUrl, nameof(request.AudioUrl));

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
        var responseText = chatCompletion.Messages[0].Text ?? "Transcription failed.";

        // Calculate Metadata & Usage
        var tokenUsage = chatCompletion.Usage?.TotalTokenCount ?? (messages.Sum(i => i.Text.Length) + responseText.Length) / 4;

        // Get cost per token from model service
        var costPerToken = _modelService.GetCostPerToken(modelId);
        var costValue = (decimal)tokenUsage * costPerToken;

        // Persist
        var sessionId = SpeechToTextId.Of(Guid.NewGuid());
        var userId = UserId.Of(request.UserId);
        var config = new SpeechToTextConfiguration(
            LanguageCode.Of(request.Language),
            includePunctuation: request.IncludePunctuation,
            detailLevel: request.DetailLevel);

        var session = SpeechToTextSession.Create(sessionId, userId, modelId, config);

        var resultId = SpeechToTextResultId.Of(Guid.NewGuid());
        var audioVo = AudioSource.Of(request.AudioUrl);
        var transcriptVo = Transcript.Of(responseText);
        var tokenCountVo = TokenCount.Of(tokenUsage);
        var costVo = CostEstimate.Of(costValue);

        var result = SpeechToTextResult.Create(
                  resultId, 
                  audioVo, 
                  transcriptVo, 
                  tokenCountVo, 
                  costVo);

        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new TranscribeAudioCommandResult(sessionId.Value, resultId.Value, responseText, modelIdStr, providerName);
    }
}
