using AiOrchestration.ValueObjects;
using AiOrchestration.Models;
using MediatR;
using Microsoft.Extensions.AI;
using SpeechToText.Data;
using SpeechToText.Models;
using SpeechToText.ValueObjects;
using SpeechToText.Enums;
using System.Runtime.CompilerServices;
using Ardalis.GuardClauses;
using AiOrchestration.Services;
using System.Text;

namespace SpeechToText.Features.StreamTranscribeAudio.V1;


internal class StreamTranscribeAudioHandler : IStreamRequestHandler<StreamTranscribeAudioCommand, string>
{
    private readonly SpeechToTextDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IAiModelService _modelService;

    public StreamTranscribeAudioHandler(SpeechToTextDbContext dbContext, IAiOrchestrator orchestrator, IAiModelService modelService)
    {
        _dbContext = dbContext;
        _orchestrator = orchestrator;
        _modelService = modelService;
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

        // Use orchestrator to get the client based on requested model criteria
        var criteria = new ModelCriteria { ModelId = request.ModelId };
        var chatClient = await _orchestrator.GetClientAsync(criteria, cancellationToken);
        
        await foreach (var update in chatClient.GetStreamingResponseAsync(messages, cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                fullTranscriptBuilder.Append(update.Text);
                tokenEstimate++;
                yield return update.Text;
            }
        }

        // Persist session after stream
        await PersistTranscriptionAsync(request, fullTranscriptBuilder.ToString(), tokenEstimate, chatClient, cancellationToken);
    }

    private async Task PersistTranscriptionAsync(StreamTranscribeAudioCommand request, string fullText, int tokenUsage, IChatClient chatClient, CancellationToken cancellationToken)
    {
        try
        {
            var sessionId = SpeechToTextId.Of(Guid.NewGuid());
            var userId = UserId.Of(request.UserId);

            
            var clientMetadata = chatClient.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata;
            var modelIdStr = clientMetadata?.DefaultModelId ?? "whisper-stream";
            var modelId = ModelId.Of(modelIdStr);
            
            var config = new SpeechToTextConfiguration(
                LanguageCode.Of(request.Language),
                includePunctuation: true,
                detailLevel: SpeechToTextDetailLevel.Detailed);

            var session = SpeechToTextSession.Create(sessionId, userId, modelId, config);

            var resultId = SpeechToTextResultId.Of(Guid.NewGuid());
            var audioVo = AudioSource.Of(request.AudioUrl);
            var transcriptVo = Transcript.Of(fullText);
            var tokenCountVo = TokenCount.Of(tokenUsage);
            
            // Get cost from model service
            var costPerToken = _modelService.GetCostPerToken(modelId);
            var costValue = (decimal)tokenUsage * costPerToken;
            var costVo = CostEstimate.Of(costValue);

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

