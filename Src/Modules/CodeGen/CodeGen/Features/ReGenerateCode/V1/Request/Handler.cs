using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using CodeGen.Data;
using CodeGen.Enums;
using CodeGen.Exceptions;
using CodeGen.Models;
using CodeGen.ValueObjects;
using Microsoft.Extensions.AI;

namespace CodeGen.Features.ReGenerateCode.V1;


internal class ReGenerateCodeHandler : ICommandHandler<ReGenerateCodeCommand, ReGenerateCodeCommandResult>
{
    private readonly CodeGenDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IChatClient _chatClient;
    private readonly IAiModelService _modelService;

    public ReGenerateCodeHandler(CodeGenDbContext dbContext, IAiModelService modelService, IAiOrchestrator orchestrator, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _modelService = modelService;
        _orchestrator = orchestrator;
        _chatClient = chatClient;
    }

    public async Task<ReGenerateCodeCommandResult> Handle(ReGenerateCodeCommand request, CancellationToken cancellationToken)
    {
        // Use orchestrator to get the client based on requested model criteria
        var criteria = new ModelCriteria { ModelId = request.ModelId };
        var chatClient = await _orchestrator.GetClientAsync(criteria, cancellationToken);

        // Get actual model info from client metadata
        var clientMetadata = chatClient.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata;
        var modelIdStr = clientMetadata?.DefaultModelId ?? "default-model";
        var providerName = clientMetadata?.ProviderName ?? "Unknown";
        var modelId = ModelId.Of(modelIdStr);

        // Load Session
        var session = await _dbContext.Sessions.FindAsync(new object[] { CodeGenId.Of(request.SessionId) }, cancellationToken);
        if (session == null) throw new CodeGenNotFoundException(request.SessionId);

        // Prepare Context
        // We want the last generated code as context
        // EF Core loading results...
        await _dbContext.Entry(session).Collection(s => s.Results).LoadAsync(cancellationToken);

        var lastResult = session.Results.OrderByDescending(r => r.GeneratedAt).FirstOrDefault();
        if (lastResult == null) throw new CodeGenResultNotFoundException(Guid.Empty); // Generic or specific

        #region Prompt
        var messages = new List<ChatMessage>
        {
            new ChatMessage(
                    role: ChatRole.System, 
                    content: $"You are an expert coder. Refine the provided code based on instructions. Language: {lastResult.Language.Value}"),
            new ChatMessage(
                    role: ChatRole.User, 
                    content: $"Original Code:\n{lastResult.Code.Value}"),
            new ChatMessage(
                    role: ChatRole.User, 
                    content : $"Instructions:\n{request.Instruction}")
        };
        #endregion

        // Call AI Model
        var chatCompletion = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        var responseText = chatCompletion.Messages[0].Text ?? string.Empty;

        // Calculate Metadata & Usage
        var tokenUsage = chatCompletion.Usage?.TotalTokenCount ?? (messages.Sum(i => i.Text.Length) + responseText.Length) / 4;

        // Get cost per token from model service
        var costPerToken = _modelService.GetCostPerToken(modelId);
        var costValue = (decimal)tokenUsage * costPerToken;

        // Update Session
        var resultId = CodeGenResultId.Of(Guid.NewGuid());
        var promptVo = CodeGenerationPrompt.Of(request.Instruction);
        var codeVo = GeneratedCode.Of(responseText);
        var tokenCountVo = TokenCount.Of(tokenUsage);
        var costVo = CostEstimate.Of(costValue);

        var newResult = CodeGenerationResult.Create(
            resultId,
            promptVo,
            codeVo,
            lastResult.Language, // Maintain same language
            lastResult.QualityLevel,
            tokenCountVo,
            costVo);

        session.AddResult(newResult);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new  ReGenerateCodeCommandResult(resultId.Value, responseText, modelIdStr, providerName);
    }
}
