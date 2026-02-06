using AiOrchestration.ValueObjects;
using MediatR;
using Meeting.Data;
using Meeting.Models;
using Meeting.ValueObjects;
using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;
using Ardalis.GuardClauses;
using AiOrchestration.Services;
using System.Text;
using AiOrchestration.Models;

namespace Meeting.Features.StreamMeetingAnalysis.V1;


internal class StreamMeetingAnalysisHandler : IStreamRequestHandler<StreamMeetingAnalysisCommand, string>
{
    private readonly MeetingDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IAiModelService _modelService;

    public StreamMeetingAnalysisHandler(MeetingDbContext dbContext, IAiOrchestrator orchestrator, IAiModelService modelService)
    {
        _dbContext = dbContext;
        _orchestrator = orchestrator;
        _modelService = modelService;
    }

    public async IAsyncEnumerable<string> Handle(StreamMeetingAnalysisCommand request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Transcript, nameof(request.Transcript));

        var systemPrompt = "You are a professional meeting assistant. Analyze the transcript for a detailed summary and action items. Stream your results.";
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, $"Transcript to analyze:\n{request.Transcript}")
        };

        var fullAnalysisBuilder = new StringBuilder();
        int tokenEstimate = 0;

        // Use orchestrator to get the client based on requested model criteria
        var criteria = new ModelCriteria { ModelId = request.ModelId };
        var chatClient = await _orchestrator.GetClientAsync(criteria, cancellationToken);
        
        await foreach (var update in chatClient.GetStreamingResponseAsync(messages, cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                fullAnalysisBuilder.Append(update.Text);
                tokenEstimate++;
                yield return update.Text;
            }
        }

        // Persist interaction after stream
        await PersistAnalysisAsync(request, fullAnalysisBuilder.ToString(), tokenEstimate, chatClient, cancellationToken);
    }

    private async Task PersistAnalysisAsync(StreamMeetingAnalysisCommand request, string fullAnalysis, int tokenUsage, IChatClient chatClient, CancellationToken cancellationToken)
    {
        try
        {
            var meetingId = MeetingId.Of(Guid.NewGuid());
            var userId = UserId.Of(request.UserId);

            
            var clientMetadata = chatClient.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata;
            var modelIdStr = clientMetadata?.DefaultModelId ?? "meeting-stream-model";
            var modelId = ModelId.Of(modelIdStr);
            
            var langCode = LanguageCode.Of("en");
            var config = new MeetingAnalysisConfiguration(true, true, langCode);

            var session = MeetingAnalysisSession.Create(meetingId, userId, modelId, config);

            var transcriptId = TranscriptId.Of(Guid.NewGuid());
            var summaryVo = TranscriptSummary.Of(fullAnalysis);
            var tokenCountVo = TokenCount.Of(tokenUsage);
            
            // Get cost from model service
            var costPerToken = _modelService.GetCostPerToken(modelId);
            var costValue = (decimal)tokenUsage * costPerToken;
            var costVo = CostEstimate.Of(costValue);

            var transcript = MeetingTranscript.Create(transcriptId, request.Transcript, summaryVo, tokenCountVo, costVo);
            session.AddTranscript(transcript);

            _dbContext.Sessions.Add(session);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // Log persistence error
        }
    }
}

