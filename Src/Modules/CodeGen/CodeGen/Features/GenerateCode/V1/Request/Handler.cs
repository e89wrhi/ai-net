using AI.Common.Core;
using AiOrchestration.ValueObjects;
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
    private readonly IChatClient _chatClient;

    public GenerateCodeHandler(CodeGenDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<GenerateCodeCommandResult> Handle(GenerateCodeCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Prompt, nameof(request.Prompt));
        Guard.Against.NullOrEmpty(request.Language, nameof(request.Language));

        // 1. Prepare AI Prompt
        var systemPrompt = $"You are an expert code generator. Generate {request.Language} code based on the user's prompt. Return ONLY the code, no explanation, no markdown tags unless requested.";

        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, request.Prompt)
        };

        // 2. Call AI
        var completion = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);
        var generatedCodeText = completion.Message.Text ?? string.Empty;

        // 3. Persist
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

        return new GenerateCodeResult(sessionId.Value, resultId.Value, generatedCodeText);
    }
}

