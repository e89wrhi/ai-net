using AiOrchestration.ValueObjects;
using MediatR;
using Microsoft.Extensions.AI;
using SpeechToText.Data;
using SpeechToText.Models;
using SpeechToText.ValueObjects;
using System.Runtime.CompilerServices;
using System.Text;

namespace SpeechToText.Features.StreamTranscribeAudio.V1;


internal class StreamTranscribeAudioHandler : IStreamRequestHandler<StreamTranscribeAudioCommand, string>
{
    private readonly SpeechToTextDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public StreamTranscribeAudioHandler(SpeechToTextDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async IAsyncEnumerable<string> Handle(StreamTranscribeAudioCommand request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.AudioUrl, nameof(request.AudioUrl));

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are a real-time transcription engine."),
            new ChatMessage(ChatRole.User, $"Transcribe: {request.AudioUrl} ({request.Language})")
        };

        var fullTranscriptBuilder = new StringBuilder();
        int tokenEstimate = 0;

        await foreach (var update in _chatClient.CompleteStreamingAsync(messages, cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                fullTranscriptBuilder.Append(update.Text);
                tokenEstimate++;
                yield return update.Text;
            }
        }

        // Persist session after stream
        await PersistTranscriptionAsync(request, fullTranscriptBuilder.ToString(), tokenEstimate, cancellationToken);
    }

    private async Task PersistTranscriptionAsync(StreamTranscribeAudioCommand request, string fullText, int tokenUsage, CancellationToken cancellationToken)
    {
        try
        {
            var sessionId = SpeechToTextId.Of(Guid.NewGuid());
            var userId = UserId.Of(Guid.NewGuid());
            var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "whisper-stream");
            var config = SpeechToTextConfiguration.Of(LanguageCode.Of(request.Language));

            var session = SpeechToTextSession.Create(sessionId, userId, modelId, config);

            var resultId = SpeechToTextResultId.Of(Guid.NewGuid());
            var audioVo = AudioSource.Of(request.AudioUrl);
            var transcriptVo = Transcript.Of(fullText);
            var tokenCountVo = TokenCount.Of(tokenUsage);
            var costVo = CostEstimate.Of(0);

            var result = SpeechToTextResult.Create(resultId, audioVo, transcriptVo, tokenCountVo, costVo);
            session.AddResult(result);

            _dbContext.Sessions.Add(session);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // Log persistence error
        }
    }
}
