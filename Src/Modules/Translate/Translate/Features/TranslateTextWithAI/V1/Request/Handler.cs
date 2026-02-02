using AI.Common.Core;
using AiOrchestration.ValueObjects;
using Microsoft.Extensions.AI;
using Translate.Data;
using Translate.Models;
using Translate.ValueObjects;

namespace Translate.Features.TranslateTextWithAI.V1;


internal class TranslateTextWithAIHandler : ICommandHandler<TranslateTextWithAICommand, TranslateTextWithAICommandResult>
{
    private readonly TranslateDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public TranslateTextWithAIHandler(TranslateDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
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

        var completion = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);
        var translatedText = completion.Message.Text ?? "Translation failed.";

        // Persist
        var sessionId = TranslateId.Of(Guid.NewGuid());
        var userId = UserId.Of(Guid.NewGuid());
        var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "translate-model");
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
