using AI.Common.Core;
using AiOrchestration.ValueObjects;
using CodeGen.Data;
using CodeGen.Enums;
using CodeGen.Models;
using Ardalis.GuardClauses;
using AiOrchestration.Services;
using CodeGen.ValueObjects;
using Microsoft.Extensions.AI;

namespace CodeGen.Features.ReGenerateCode.V1;


internal class ReGenerateCodeHandler : ICommandHandler<ReGenerateCodeCommand, ReGenerateCodeCommandResult>
{
    private readonly CodeGenDbContext _dbContext;
    private readonly IAiOrchestrator _chatClient;

    public ReGenerateCodeHandler(CodeGenDbContext dbContext, IAiOrchestrator chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<ReGenerateCodeCommandResult> Handle(ReGenerateCodeCommand request, CancellationToken cancellationToken)
    {
        // Load Session
        var session = await _dbContext.Sessions.FindAsync(new object[] { CodeGenId.Of(request.SessionId) }, cancellationToken);
        if (session == null) throw new CodeGen.Exceptions.CodeGenNotFoundException(request.SessionId);

        // Prepare Context
        // We want the last generated code as context
        // EF Core loading results...
        await _dbContext.Entry(session).Collection(s => s.Results).LoadAsync(cancellationToken);

        var lastResult = session.Results.OrderByDescending(r => r.GeneratedAt).FirstOrDefault();
        if (lastResult == null) throw new CodeGen.Exceptions.CodeGenResultNotFoundException(Guid.Empty); // Generic or specific

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, $"You are an expert coder. Refine the provided code based on instructions. Language: {lastResult.Language.Value}"),
            new ChatMessage(ChatRole.User, $"Original Code:\n{lastResult.Code.Value}"),
            new ChatMessage(ChatRole.User, $"Instructions:\n{request.Instruction}")
        };

        // Use chatClient to get the best client
        var chatClient = await _chatClient.GetClientAsync(cancellationToken: cancellationToken);

        // Call AI
        var completion = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        var refinedCodeText = completion.Messages[0].Text ?? string.Empty;

        // Update Session
        var resultId = CodeGenResultId.Of(Guid.NewGuid());
        var promptVo = CodeGenerationPrompt.Of(request.Instruction);
        var codeVo = GeneratedCode.Of(refinedCodeText);
        var tokenCountVo = TokenCount.Of(completion.Usage?.TotalTokenCount ?? 0);
        var costVo = CostEstimate.Of(0);

        var newResult = CodeGenerationResult.Create(
            resultId,
            promptVo,
            codeVo,
            lastResult.Language, // Maintain same language
            CodeQualityLevel.High,
            tokenCountVo,
            costVo);

        session.AddResult(newResult);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new  ReGenerateCodeCommandResult(resultId.Value, refinedCodeText);
    }
}
