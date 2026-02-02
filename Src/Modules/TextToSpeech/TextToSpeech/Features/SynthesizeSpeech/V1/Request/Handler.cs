using AI.Common.Core;
using AiOrchestration.ValueObjects;
using Microsoft.Extensions.AI;
using TextToSpeech.Data;
using TextToSpeech.Models;
using TextToSpeech.ValueObjects;

namespace TextToSpeech.Features.SynthesizeSpeech.V1;


internal class SynthesizeSpeechHandler : ICommandHandler<SynthesizeSpeechCommand, SynthesizeSpeechCommandResult>
{
    private readonly TextToSpeechDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public SynthesizeSpeechHandler(TextToSpeechDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<SynthesizeSpeechCommandResult> Handle(SynthesizeSpeechCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Text, nameof(request.Text));

        // Use IChatClient to orchestrate or log the synthesis intent
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are a speech synthesis orchestrator."),
            new ChatMessage(ChatRole.User, $"Synthesize this text using {request.Voice} voice at {request.Speed} speed in {request.Language}: {request.Text}")
        };

        var completion = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);

        // Mock Audio URL (In real implementation, this would call a TTS service like OpenAI TTS or Azure Speech)
        var audioUrl = $"https://ai-audio-storage.com/{Guid.NewGuid()}.mp3";

        // Persist
        var sessionId = TextToSpeechId.Of(Guid.NewGuid());
        var userId = UserId.Of(Guid.NewGuid()); // Mock
        var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "tts-1-hd");
        var config = new TextToSpeechConfiguration(request.Voice, request.Speed, LanguageCode.Of(request.Language));

        var session = TextToSpeechSession.Create(sessionId, userId, modelId, config);

        var resultId = TextToSpeechResultId.Of(Guid.NewGuid());
        var speechVo = SynthesizedSpeech.Of(audioUrl);
        var tokenCountVo = TokenCount.Of(request.Text.Length); // For TTS, tokens might be char count or similar
        var costVo = CostEstimate.Of(0.015m);

        var result = TextToSpeechResult.Create(resultId, request.Text, speechVo, tokenCountVo, costVo);
        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SynthesizeSpeechCommandResult(sessionId.Value, resultId.Value, audioUrl);
    }
}
