using AiOrchestration.ValueObjects;
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
    private readonly IChatClient _chatClient;

    public StreamTranslateTextHandler(TranslateDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
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

        await foreach (var update in _chatClient.CompleteStreamingAsync(messages, cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                fullTranslationBuilder.Append(update.Text);
                tokenEstimate++;
                yield return update.Text;
            }
        }

        // Persist session after stream completion
        await PersistTranslationAsync(request, fullTranslationBuilder.ToString(), tokenEstimate, cancellationToken);
    }

    private async Task PersistTranslationAsync(StreamTranslateTextCommand request, string fullText, int tokenUsage, CancellationToken cancellationToken)
    {
        try
        {
            var sessionId = TranslateId.Of(Guid.NewGuid());
            var userId = UserId.Of(Guid.NewGuid());
            var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "translate-stream-model");
            var config = new TranslationConfiguration(
                LanguageCode.Of(request.SourceLanguage),
                LanguageCode.Of(request.TargetLanguage),
                request.DetailLevel);

            var session = TranslationSession.Create(sessionId, userId, modelId, config);

            var resultId = TranslateResultId.Of(Guid.NewGuid());
            var translatedTextVo = TranslatedText.Of(fullText);
            var tokenCountVo = TokenCount.Of(tokenUsage);
            var costVo = CostEstimate.Of(0);

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
