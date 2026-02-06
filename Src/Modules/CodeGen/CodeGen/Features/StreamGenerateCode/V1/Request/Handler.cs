using AiOrchestration.ValueObjects;
using AiOrchestration.Models;
using CodeGen.Data;
using CodeGen.Enums;
using CodeGen.Models;
using CodeGen.ValueObjects;
using MediatR;
using Ardalis.GuardClauses;
using AiOrchestration.Services;
using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;
using System.Text;

namespace CodeGen.Features.StreamGenerateCode.V1;


internal class StreamGenerateCodeHandler : IStreamRequestHandler<StreamGenerateCodeCommand, string>
{
    private readonly CodeGenDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IAiModelService _modelService;

    public StreamGenerateCodeHandler(CodeGenDbContext dbContext, IAiOrchestrator orchestrator, IAiModelService modelService)
    {
        _dbContext = dbContext;
        _orchestrator = orchestrator;
        _modelService = modelService;
    }

    public async IAsyncEnumerable<string> Handle(StreamGenerateCodeCommand request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Prompt, nameof(request.Prompt));

        var systemPrompt = $"You are an expert code generator. Generate {request.Language} code based on the user's prompt. Return ONLY the code.";
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, request.Prompt)
        };

        var fullCodeBuilder = new StringBuilder();
        int tokenEstimate = 0;

        // Use orchestrator to get the client based on requested model criteria
        var criteria = new ModelCriteria { ModelId = request.ModelId };
        var chatClient = await _orchestrator.GetClientAsync(criteria, cancellationToken);
        
        await foreach (var update in chatClient.GetStreamingResponseAsync(messages, cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                fullCodeBuilder.Append(update.Text);
                tokenEstimate++;
                yield return update.Text;
            }
        }

        // Persist
        await PersistResultAsync(request, fullCodeBuilder.ToString(), tokenEstimate, chatClient, cancellationToken);
    }

    private async Task PersistResultAsync(StreamGenerateCodeCommand request, string fullCode, int tokenUsage, IChatClient chatClient, CancellationToken cancellationToken)
    {
        try
        {
            var sessionId = CodeGenId.Of(Guid.NewGuid());
            var userId = UserId.Of(request.UserId);

            
            var clientMetadata = chatClient.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata;
            var modelIdStr = clientMetadata?.DefaultModelId ?? "codegen-stream-model";
            var modelId = ModelId.Of(modelIdStr);
            
            var config = new CodeGenerationConfiguration(
                Temperature.Of(0.07f),
                TokenCount.Of(tokenUsage),
                style: CodeStyle.Enterprise,
                includeComments: true);

            var session = CodeGenerationSession.Create(sessionId, userId, modelId, config);

            var resultId = CodeGenResultId.Of(Guid.NewGuid());
            var promptVo = CodeGenerationPrompt.Of(request.Prompt);
            var codeVo = GeneratedCode.Of(fullCode);
            var languageVo = ProgrammingLanguage.Of(request.Language);
            var tokenCountVo = TokenCount.Of(tokenUsage);
            
            // Get cost from model service
            var costPerToken = _modelService.GetCostPerToken(modelId);
            var costValue = (decimal)tokenUsage * costPerToken;
            var costVo = CostEstimate.Of(costValue);

            var result = CodeGenerationResult.Create(
                resultId,
                promptVo,
                codeVo,
                languageVo,
                qualityLevel: CodeQualityLevel.Optimized,
                tokenCountVo,
                costVo);

            session.AddResult(result);

            _dbContext.Sessions.Add(session);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // Log error
        }
    }
}

