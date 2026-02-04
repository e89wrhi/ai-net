using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using CodeGen.Data;
using CodeGen.Enums;
using CodeGen.Features.ReGenerateCode.V1;
using CodeGen.Models;
using CodeGen.ValueObjects;
using Microsoft.Extensions.AI;

namespace CodeGen.Features.GenerateCode.V1;

internal class GenerateCodeHandler : ICommandHandler<GenerateCodeCommand, GenerateCodeCommandResult>
{
    private readonly CodeGenDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IChatClient _chatClient;
    private readonly IAiModelService _modelService;

    public GenerateCodeHandler(CodeGenDbContext dbContext, IAiModelService modelService, IAiOrchestrator orchestrator, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _modelService = modelService;
        _orchestrator = orchestrator;
        _chatClient = chatClient;
    }

    public async Task<GenerateCodeCommandResult> Handle(GenerateCodeCommand request, CancellationToken cancellationToken)
    {
        #region Prompt
        var messages = new List<ChatMessage>
        {
            new ChatMessage(
                    role: ChatRole.System, 
                    content: $"You are an expert code generator. Generate {request.Language} code based on the user's prompt. Return ONLY the code, no explanation, no markdown tags unless requested."),
            new ChatMessage(
                    role: ChatRole.User, 
                    content : request.Prompt)
        };
        #endregion

        Guard.Against.NullOrEmpty(request.Prompt, nameof(request.Prompt));
        Guard.Against.NullOrEmpty(request.Language, nameof(request.Language));

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
        var responseText = chatCompletion.Messages[0].Text ?? string.Empty;

        // Calculate Metadata & Usage
        var tokenUsage = chatCompletion.Usage?.TotalTokenCount ?? (messages.Sum(i => i.Text.Length) + responseText.Length) / 4;

        // Get cost per token from model service
        var costPerToken = _modelService.GetCostPerToken(modelId);
        var costValue = (decimal)tokenUsage * costPerToken;

        // Persist
        var sessionId = CodeGenId.Of(Guid.NewGuid());
        var userId = UserId.Of(request.UserId);
        var config = new CodeGenerationConfiguration(
            Temperature.Of(0),
            TokenCount.Of(tokenUsage),
            CodeStyle.Minimal,
            includeComments: false);

        var session = CodeGenerationSession.Create(sessionId, userId, modelId, config);

        var resultId = CodeGenResultId.Of(Guid.NewGuid());
        var promptVo = CodeGenerationPrompt.Of(request.Prompt);
        var codeVo = GeneratedCode.Of(responseText);
        var languageVo = ProgrammingLanguage.Of(request.Language);
        var tokenCountVo = TokenCount.Of(tokenUsage);
        var costVo = CostEstimate.Of(costValue);

        var result = CodeGenerationResult.Create(
            resultId,
            promptVo,
            codeVo,
            languageVo,
            CodeQualityLevel.Optimized,
            tokenCountVo,
            costVo);

        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new GenerateCodeCommandResult(sessionId.Value, resultId.Value, responseText, modelIdStr, providerName);
    }
}

