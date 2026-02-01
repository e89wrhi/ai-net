using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using Summary.Data;
using Summary.Models;
using Summary.ValueObjects;
using Summary.Enums;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace Summary.Features.SummarizeTextWithAI.V1;

public record SummarizeTextWithAICommand(string Text, SummaryDetailLevel DetailLevel, string Language) : ICommand<SummarizeTextWithAIResult>;

public record SummarizeTextWithAIResult(Guid SessionId, Guid ResultId, string Summary);

public class SummarizeTextWithAIEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/summary/summarize",
                async (SummarizeTextWithAIRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new SummarizeTextWithAICommand(request.Text, request.DetailLevel, request.Language);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new SummarizeTextWithAIResponseDto(result.SessionId, result.ResultId, result.Summary));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("SummarizeTextWithAI")
            .WithApiVersionSet(builder.NewApiVersionSet("Summary").Build())
            .Produces<SummarizeTextWithAIResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Summarize Text")
            .WithDescription("Uses AI to generate a summary of the provided text based on the specified detail level.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public record SummarizeTextWithAIRequestDto(string Text, SummaryDetailLevel DetailLevel, string Language);
public record SummarizeTextWithAIResponseDto(Guid SessionId, Guid ResultId, string Summary);

internal class SummarizeTextWithAIHandler : ICommandHandler<SummarizeTextWithAICommand, SummarizeTextWithAIResult>
{
    private readonly SummaryDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public SummarizeTextWithAIHandler(SummaryDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<SummarizeTextWithAIResult> Handle(SummarizeTextWithAICommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Text, nameof(request.Text));

        var prompt = $"Please provide a {request.DetailLevel} summary of the following text in {request.Language}:\n\n{request.Text}";
        
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are a professional summarization assistant."),
            new ChatMessage(ChatRole.User, prompt)
        };

        var completion = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);
        var summaryText = completion.Message.Text ?? "Summary generation failed.";

        // Persist
        var sessionId = SummaryId.Of(Guid.NewGuid());
        var userId = UserId.Of(Guid.NewGuid()); // Mock
        var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "summary-model");
        var config = new TextSummaryConfiguration(request.DetailLevel, LanguageCode.Of(request.Language));

        var session = TextSummarySession.Create(sessionId, userId, modelId, config);

        var resultId = SummaryResultId.Of(Guid.NewGuid());
        var summaryVo = SummaryText.Of(summaryText);
        var tokenCountVo = TokenCount.Of(completion.Usage?.TotalTokenCount ?? 0);
        var costVo = CostEstimate.Of(0);

        var result = TextSummaryResult.Create(resultId, request.Text, summaryVo, tokenCountVo, costVo);
        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SummarizeTextWithAIResult(sessionId.Value, resultId.Value, summaryText);
    }
}
