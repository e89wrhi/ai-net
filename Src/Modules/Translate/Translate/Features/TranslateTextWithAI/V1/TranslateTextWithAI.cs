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

namespace Translate.Features.TranslateTextWithAI.V1;

public record TranslateTextWithAICommand(string Text, string SourceLanguage, string TargetLanguage, TranslationDetailLevel DetailLevel) : ICommand<TranslateTextWithAIResult>;

public record TranslateTextWithAIResult(Guid SessionId, Guid ResultId, string TranslatedText);

public class TranslateTextWithAIEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/translate/translate",
                async (TranslateTextWithAIRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new TranslateTextWithAICommand(request.Text, request.SourceLanguage, request.TargetLanguage, request.DetailLevel);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new TranslateTextWithAIResponseDto(result.SessionId, result.ResultId, result.TranslatedText));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("TranslateTextWithAI")
            .WithApiVersionSet(builder.NewApiVersionSet("Translate").Build())
            .Produces<TranslateTextWithAIResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Translate Text")
            .WithDescription("Uses AI to translate text from a source language to a target language.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public record TranslateTextWithAIRequestDto(string Text, string SourceLanguage, string TargetLanguage, TranslationDetailLevel DetailLevel);
public record TranslateTextWithAIResponseDto(Guid SessionId, Guid ResultId, string TranslatedText);

internal class TranslateTextWithAIHandler : ICommandHandler<TranslateTextWithAICommand, TranslateTextWithAIResult>
{
    private readonly TranslateDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public TranslateTextWithAIHandler(TranslateDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<TranslateTextWithAIResult> Handle(TranslateTextWithAICommand request, CancellationToken cancellationToken)
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
