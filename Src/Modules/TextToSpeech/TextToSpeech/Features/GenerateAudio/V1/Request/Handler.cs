using AI.Common.Core;
using AiOrchestration.ValueObjects;
using Microsoft.Extensions.AI;
using TextToSpeech.Data;
using TextToSpeech.Enums;
using TextToSpeech.Models;
using TextToSpeech.ValueObjects;
using Ardalis.GuardClauses;
using AiOrchestration.Services;

namespace TextToSpeech.Features.GenerateAudio.V1;


internal class GenerateAudioWithAIHandler : ICommandHandler<GenerateAudioWithAICommand, GenerateAudioWithAICommandResult>
{
    private readonly TextToSpeechDbContext _dbContext;
    private readonly IAiOrchestrator _chatClient;

    public GenerateAudioWithAIHandler(TextToSpeechDbContext dbContext, IAiOrchestrator chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<GenerateAudioWithAICommandResult> Handle(GenerateAudioWithAICommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Text, nameof(request.Text));

        // Mock Logic for AI Voice Generation
        var audioUrl = $"https://cdn.ai-voices.com/gen/{Guid.NewGuid()}.wav";

        // Persist
        var sessionId = TextToSpeechId.Of(Guid.NewGuid());
        var userId = UserId.Of(Guid.NewGuid());
        var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "tts-expressive-1");
        var config = new TextToSpeechConfiguration(request.Voice, SpeechSpeed.Normal, LanguageCode.Of("en"));

        var session = TextToSpeechSession.Create(sessionId, userId, modelId, config);

        var resultId = TextToSpeechResultId.Of(Guid.NewGuid());
        var speechVo = SynthesizedSpeech.Of(audioUrl);
        var tokenCountVo = TokenCount.Of(request.Text.Length);
        var costVo = CostEstimate.Of(0.02m);

        var result = TextToSpeechResult.Create(resultId, request.Text, speechVo, tokenCountVo, costVo);
        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new GenerateAudioWithAICommandResult(sessionId.Value, audioUrl);
    }
}
