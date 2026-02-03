using AI.Common.Core;
using AiOrchestration.ValueObjects;
using CodeGen.Data;
using CodeGen.Enums;
using CodeGen.Features.ReGenerateCode.V1;
using CodeGen.Models;
using CodeGen.ValueObjects;
using Ardalis.GuardClauses;
using AiOrchestration.Services;
using Microsoft.Extensions.AI;

namespace CodeGen.Features.GenerateCode.V1;

internal class GenerateCodeHandler : ICommandHandler<GenerateCodeCommand, GenerateCodeCommandResult>
{
    private readonly CodeGenDbContext _dbContext;
    private readonly IAiOrchestrator _chatClient;

    public GenerateCodeHandler(CodeGenDbContext dbContext, IAiOrchestrator chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<GenerateCodeCommandResult> Handle(GenerateCodeCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Prompt, nameof(request.Prompt));
        Guard.Against.NullOrEmpty(request.Language, nameof(request.Language));

        // Prepare AI Prompt
        var systemPrompt = $"You are an expert code generator. Generate {request.Language} code based on the user's prompt. Return ONLY the code, no explanation, no markdown tags unless requested.";

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, request.Prompt)
        };

        // Use chatClient to get the best client
        var chatClient = await _chatClient.GetClientAsync(cancellationToken: cancellationToken);
        // Call AI
        var completion = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        var generatedCodeText = completion.Messages[0].Text ?? string.Empty;

        // Persist
        var sessionId = CodeGenId.Of(Guid.NewGuid());
        var userId = UserId.Of(Guid.NewGuid()); // Mock user
        var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "codegen-model");
        var config = CodeGenerationConfiguration.Of("standard");

        var session = CodeGenerationSession.Create(sessionId, userId, modelId, config);

        var resultId = CodeGenResultId.Of(Guid.NewGuid());
        var promptVo = CodeGenerationPrompt.Of(request.Prompt);
        var codeVo = GeneratedCode.Of(generatedCodeText);
        var languageVo = ProgrammingLanguage.Of(request.Language);
        var tokenCountVo = TokenCount.Of(completion.Usage?.TotalTokenCount ?? 0);
        var costVo = CostEstimate.Of(0);

        var result = CodeGenerationResult.Create(
            resultId,
            promptVo,
            codeVo,
            languageVo,
            CodeQualityLevel.High,
            tokenCountVo,
            costVo);

        session.AddResult(result);

        _dbContext.Sessions.Add(session);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new GenerateCodeCommandResult(sessionId.Value, resultId.Value, generatedCodeText);
    }
}

