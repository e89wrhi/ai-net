using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;
using System.Text;
using Translate.Data;
using Translate.Models;
using Translate.ValueObjects;

namespace Translate.Features.StreamTranslateText.V1;


internal class StreamTranslateTextHandler : IStreamRequestHandler<StreamTranslateTextCommand, string>
{
    private readonly TranslateDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IAiModelService _modelService;

    public StreamTranslateTextHandler(TranslateDbContext dbContext, IAiOrchestrator orchestrator, IAiModelService modelService)
    {
        _dbContext = dbContext;
        _orchestrator = orchestrator;
        _modelService = modelService;
    }

    public async IAsyncEnumerable<string> Handle(StreamTranslateTextCommand request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Text, nameof(request.Text));

        var prompt = $"Translate the following text from {request.SourceLanguage} to {request.TargetLanguage}. Detail level: {request.DetailLevel}.\n\nText: {request.Text}";
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are a professional translator."),
            new ChatMessage(ChatRole.User, prompt)
        };

        var fullTranslationBuilder = new StringBuilder();
        int tokenEstimate = 0;

        // Use orchestrator to get the client based on requested model criteria
        var criteria = new ModelCriteria { ModelId = request.ModelId };
        var chatClient = await _orchestrator.GetClientAsync(criteria, cancellationToken);
        
        await foreach (var update in chatClient.GetStreamingResponseAsync(messages, cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                fullTranslationBuilder.Append(update.Text);
                tokenEstimate++;
                yield return update.Text;
            }
        }

        // Persist session after stream completion
        await PersistTranslationAsync(request, fullTranslationBuilder.ToString(), tokenEstimate, chatClient, cancellationToken);
    }

    private async Task PersistTranslationAsync(StreamTranslateTextCommand request, string fullText, int tokenUsage, IChatClient chatClient, CancellationToken cancellationToken)
    {
        try
        {
            var sessionId = TranslateId.Of(Guid.NewGuid());
            var userId = UserId.Of(request.UserId);

            
            var clientMetadata = chatClient.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata;
            var modelIdStr = clientMetadata?.DefaultModelId ?? "translate-stream-model";
            var modelId = ModelId.Of(modelIdStr);
            
            var config = new TranslationConfiguration(
                LanguageCode.Of(request.SourceLanguage),
                LanguageCode.Of(request.TargetLanguage),
                request.DetailLevel);

            var session = TranslationSession.Create(sessionId, userId, modelId, config);

            var resultId = TranslateResultId.Of(Guid.NewGuid());
            var translatedTextVo = TranslatedText.Of(fullText);
            var tokenCountVo = TokenCount.Of(tokenUsage);
            
            // Get cost from model service
            var costPerToken = _modelService.GetCostPerToken(modelId);
            var costValue = (decimal)tokenUsage * costPerToken;
            var costVo = CostEstimate.Of(costValue);

            var result = TranslationResult.Create(resultId, request.Text, translatedTextVo, tokenCountVo, costVo);
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

