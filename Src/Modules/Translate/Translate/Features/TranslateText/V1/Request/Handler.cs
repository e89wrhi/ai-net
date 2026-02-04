using AI.Common.Core;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using Microsoft.Extensions.AI;
using Translate.Data;
using Translate.Models;
using Translate.ValueObjects;

namespace Translate.Features.TranslateText.V1;


internal class TranslateTextWithAIHandler : ICommandHandler<TranslateTextWithAICommand, TranslateTextWithAICommandResult>
{
    private readonly TranslateDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;

    public TranslateTextWithAIHandler(TranslateDbContext dbContext, IAiOrchestrator orchestrator)
    {
        _dbContext = dbContext;
        _orchestrator = orchestrator;
    }

    public async Task<TranslateTextWithAICommandResult> Handle(TranslateTextWithAICommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Text, nameof(request.Text));

        var prompt = $"Translate the following text from {request.SourceLanguage} to {request.TargetLanguage}. Detail level: {request.DetailLevel}.\n\nText: {request.Text}";

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are a professional translator."),
            new ChatMessage(ChatRole.User, prompt)
        };

        // Use orchestrator to get the best client
        var criteria = new ModelCriteria { ModelId = request.ModelId };
        var orchestrator = await _orchestrator.GetClientAsync(criteria, cancellationToken: cancellationToken);
        var completion = await orchestrator.GetResponseAsync(messages, cancellationToken: cancellationToken);
        var translatedText = completion.Messages[0].Text ?? "Translation failed.";

        // Persist
        var sessionId = TranslateId.Of(Guid.NewGuid());
        var userId = UserId.Of(Guid.NewGuid());
        var modelId = ModelId.Of(_orchestrator.Metadata.ModelId ?? "translate-model");
        var config = new TranslationConfiguration(
            LanguageCode.Of(request.SourceLanguage),
            LanguageCode.Of(request.TargetLanguage),
            request.DetailLevel);

        var session = TranslationSession.Create(sessionId, userId, modelId, config);

        var resultId = TranslateResultId.Of(Guid.NewGuid());
        var translatedTextVo = TranslatedText.Of(translatedText);
        var tokenCountVo = TokenCount.Of(completion.Usage?.TotalTokenCount ?? 0);
        var costVo = CostEstimate.Of(0);

        var result = TranslationResult.Create(resultId, request.Text, translatedTextVo, tokenCountVo, costVo);
        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new TranslateTextWithAIResult(sessionId.Value, resultId.Value, translatedText);
    }
}
