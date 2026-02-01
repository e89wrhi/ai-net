using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using AutoComplete.Data;
using AutoComplete.Models;
using AutoComplete.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace AutoComplete.Features.GenerateAICompletion.V1;

public record GenerateAICompletionCommand(Guid UserId, string Prompt) : ICommand<GenerateAICompletionResult>;

public record GenerateAICompletionResult(string Completion, int TokensUsed, decimal EstimatedCost);

public record GenerateAICompletionRequestDto(string Prompt);
public record GenerateAICompletionResponseDto(string Completion, int TokensUsed, decimal EstimatedCost);

public class GenerateAICompletionEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/autocomplete/generate",
                async (GenerateAICompletionRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // In a real scenario, extract UserId from Claims
                    // var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var userId = Guid.NewGuid(); // Mock User ID for now

                    var command = new GenerateAICompletionCommand(userId, request.Prompt);
                    var result = await mediator.Send(command, cancellationToken);

                    var response = new GenerateAICompletionResponseDto(result.Completion, result.TokensUsed, result.EstimatedCost);

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GenerateAICompletion")
            .WithApiVersionSet(builder.NewApiVersionSet("AutoComplete").Build())
            .Produces<GenerateAICompletionResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Generate AI Completion")
            .WithDescription("Generates text completion using an AI model and stores the interaction history.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

internal class GenerateAICompletionHandler : ICommandHandler<GenerateAICompletionCommand, GenerateAICompletionResult>
{
    private readonly IChatClient _chatClient;
    private readonly AutocompleteDbContext _dbContext;

    public GenerateAICompletionHandler(IChatClient chatClient, AutocompleteDbContext dbContext)
    {
        _chatClient = chatClient;
        _dbContext = dbContext;
    }

    public async Task<GenerateAICompletionResult> Handle(GenerateAICompletionCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Prompt, nameof(request.Prompt));

        // 1. Call AI Model
        var chatCompletion = await _chatClient.CompleteAsync(request.Prompt, cancellationToken: cancellationToken);
        var responseText = chatCompletion.Message.Text ?? string.Empty;

        // 2. Calculate Metadata
        // Use usage data from the provider if available, otherwise estimate
        var tokenUsage = chatCompletion.Usage?.TotalTokenCount ?? (request.Prompt.Length + responseText.Length) / 4;
        
        // Simple cost estimation logic (example: $0.002 per 1k tokens)
        var costValue = (decimal)(tokenUsage * 0.000002); 

        // 3. Persist Interaction
        var sessionId = AutoCompleteId.Of(Guid.NewGuid());
        var userId = UserId.Of(request.UserId);
        
        // Use ModelId from metadata or fallback
        var modelIdStr = _chatClient.Metadata.ModelId ?? "default-model";
        var modelId = ModelId.Of(modelIdStr);
        
        var config = AutoCompleteConfiguration.Of("standard");

        // Create Session Aggregate
        var session = AutoCompleteSession.Create(sessionId, userId, modelId, config);

        // Add Request to Session
        var requestId = AutoCompleteRequestId.Of(Guid.NewGuid());
        
        // Assuming ValueObject.Of(string) pattern
        // If these VOs expect different types, this will need adjustment. 
        // Based on typical DDD patterns in this codebase (Of(primitive)).
        var promptVo = AutoCompletePrompt.Of(request.Prompt); 
        var suggestionVo = AutoCompleteSuggestion.Of(responseText);
        var tokenCountVo = TokenCount.Of(tokenUsage);
        var costVo = CostEstimate.Of(costValue);

        var autoCompleteRequest = AutoCompleteRequest.Create(
            requestId, 
            promptVo, 
            suggestionVo, 
            tokenCountVo, 
            costVo);

        session.AddRequest(autoCompleteRequest);

        // Save to Database
        _dbContext.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new GenerateAICompletionResult(responseText, tokenUsage, costValue);
    }
}
