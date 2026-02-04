using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using Meeting.Data;
using Meeting.Models;
using Meeting.ValueObjects;
using Microsoft.Extensions.AI;

namespace Meeting.Features.ExtractActionItems.V1;


internal class ExtractActionItemsHandler : ICommandHandler<ExtractActionItemsCommand, ExtractActionItemsCommandResult>
{
    private readonly MeetingDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IChatClient _chatClient;
    private readonly IAiModelService _modelService;

    public ExtractActionItemsHandler(MeetingDbContext dbContext, IAiModelService modelService, IAiOrchestrator orchestrator, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _modelService = modelService;
        _orchestrator = orchestrator;
        _chatClient = chatClient;
    }

    public async Task<ExtractActionItemsCommandResult> Handle(ExtractActionItemsCommand request, CancellationToken cancellationToken)
    {
        #region Prompt
        var messages = new List<ChatMessage>
        {
            new ChatMessage(
                    role: ChatRole.System, 
                    content: "You are an efficiency expert. Extract a numbered list of ONLY specific action items, the responsible person (if mentioned), and any deadlines from the transcript."),
            new ChatMessage(
                    role: ChatRole.User, 
                    content : $"Transcript:\n{request.Transcript}")
        };
        #endregion

        Guard.Against.NullOrEmpty(request.Transcript, nameof(request.Transcript));

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
        var responseText = chatCompletion.Messages[0].Text ?? "No action items found.";

        // Calculate Metadata & Usage
        var tokenUsage = chatCompletion.Usage?.TotalTokenCount ?? (messages.Sum(i => i.Text.Length) + responseText.Length) / 4;

        // Get cost per token from model service
        var costPerToken = _modelService.GetCostPerToken(modelId);
        var costValue = (decimal)tokenUsage * costPerToken;

        // Persist
        var meetingId = MeetingId.Of(Guid.NewGuid());
        var userId = UserId.Of(request.UserId);
        var langCode = LanguageCode.Of("en");
        var config = new MeetingAnalysisConfiguration(true, false, langCode);

        var session = MeetingAnalysisSession.Create(meetingId, userId, modelId, config);

        var transcriptId = TranscriptId.Of(Guid.NewGuid());
        var summaryVo = TranscriptSummary.Of(responseText); // Using Summary VO to store action items for now
        var tokenCountVo = TokenCount.Of(tokenUsage);
        var costVo = CostEstimate.Of(costValue);

        var transcript = MeetingTranscript.Create(
            transcriptId, 
            request.Transcript, 
            summaryVo, 
            tokenCountVo, 
            costVo);

        session.AddTranscript(transcript);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ExtractActionItemsCommandResult(meetingId.Value, responseText, modelIdStr, providerName);
    }
}
