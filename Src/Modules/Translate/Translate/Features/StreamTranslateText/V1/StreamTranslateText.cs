using System.Runtime.CompilerServices;
using System.Text;
using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using Translate.Data;
using Translate.Models;
using Translate.ValueObjects;
using Translate.Enums;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace Translate.Features.StreamTranslateText.V1;

public record StreamTranslateTextCommand(string Text, string SourceLanguage, string TargetLanguage, TranslationDetailLevel DetailLevel) : IStreamRequest<string>;

public class StreamTranslateTextEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/translate/translate-stream",
                (StreamTranslateTextRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    return mediator.CreateStream(new StreamTranslateTextCommand(request.Text, request.SourceLanguage, request.TargetLanguage, request.DetailLevel), cancellationToken);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StreamTranslateText")
            .WithApiVersionSet(builder.NewApiVersionSet("Translate").Build())
            .Produces<IAsyncEnumerable<string>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Stream Text Translation")
            .WithDescription("Streams the translated text from a source language to a target language.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public record StreamTranslateTextRequestDto(string Text, string SourceLanguage, string TargetLanguage, TranslationDetailLevel DetailLevel);

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
