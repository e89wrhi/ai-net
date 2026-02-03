using AI.Common.Core;
using AiOrchestration.ValueObjects;
using Microsoft.Extensions.AI;
using SpeechToText.Data;
using SpeechToText.Models;
using Ardalis.GuardClauses;
using AiOrchestration.Services;
using SpeechToText.ValueObjects;

namespace SpeechToText.Features.TranscribeAudio.V1;


internal class TranscribeAudioHandler : ICommandHandler<TranscribeAudioCommand, TranscribeAudioCommandResult>
{
    private readonly SpeechToTextDbContext _dbContext;
    private readonly IAiOrchestrator _chatClient;

    public TranscribeAudioHandler(SpeechToTextDbContext dbContext, IAiOrchestrator chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<TranscribeAudioCommandResult> Handle(TranscribeAudioCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.AudioUrl, nameof(request.AudioUrl));

        // Multi-modal models can process audio if supported, 
        // otherwise this serves as an orchestration point.
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are a transcription engine."),
            new ChatMessage(ChatRole.User, $"Please transcribe the audio at this URL: {request.AudioUrl} in language: {request.Language}")
        };

        // Use chatClient to get the best client
        var chatClient = await _chatClient.GetClientAsync(cancellationToken: cancellationToken);
        var completion = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        var transcriptText = completion.Messages[0].Text ?? "Transcription failed.";

        // Persist
        var sessionId = SpeechToTextId.Of(Guid.NewGuid());
        var userId = UserId.Of(Guid.NewGuid());
        var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "whisper-v3");
        var config = SpeechToTextConfiguration.Of(LanguageCode.Of(request.Language));

        var session = SpeechToTextSession.Create(sessionId, userId, modelId, config);

        var resultId = SpeechToTextResultId.Of(Guid.NewGuid());
        var audioVo = AudioSource.Of(request.AudioUrl);
        var transcriptVo = Transcript.Of(transcriptText);
        var tokenCountVo = TokenCount.Of(completion.Usage?.TotalTokenCount ?? 0);
        var costVo = CostEstimate.Of(0);

        var result = SpeechToTextResult.Create(resultId, audioVo, transcriptVo, tokenCountVo, costVo);
        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new TranscribeAudioCommandResult(sessionId.Value, resultId.Value, transcriptText);
    }
}
