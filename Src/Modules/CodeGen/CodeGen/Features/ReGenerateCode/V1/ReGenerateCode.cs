using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using CodeGen.Data;
using CodeGen.Enums;
using CodeGen.Models;
using CodeGen.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace CodeGen.Features.ReGenerateCode.V1;

public record ReGenerateCodeCommand(Guid SessionId, string Instruction) : ICommand<ReGenerateCodeResult>;

public record ReGenerateCodeResult(Guid ResultId, string Code);

public class ReGenerateCodeEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/codegen/regenerate",
                async (ReGenerateCodeRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new ReGenerateCodeCommand(request.SessionId, request.Instruction);
                    var result = await mediator.Send(command, cancellationToken);

                    return Results.Ok(new ReGenerateCodeResponseDto(result.ResultId, result.Code));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("ReGenerateCode")
            .WithApiVersionSet(builder.NewApiVersionSet("CodeGen").Build())
            .Produces<ReGenerateCodeResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Re-generate Code")
            .WithDescription("Re-generates or refines code based on instructions and previous session state.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public record ReGenerateCodeRequestDto(Guid SessionId, string Instruction);
public record ReGenerateCodeResponseDto(Guid ResultId, string Code);

internal class ReGenerateCodeHandler : ICommandHandler<ReGenerateCodeCommand, ReGenerateCodeResult>
{
    private readonly CodeGenDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public ReGenerateCodeHandler(CodeGenDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<ReGenerateCodeResult> Handle(ReGenerateCodeCommand request, CancellationToken cancellationToken)
    {
        // 1. Load Session
        var session = await _dbContext.Sessions.FindAsync(new object[] { CodeGenId.Of(request.SessionId) }, cancellationToken);
        if (session == null) throw new CodeGen.Exceptions.CodeGenNotFoundException(request.SessionId);

        // 2. Prepare Context
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

        // 3. Call AI
        var completion = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);
        var refinedCodeText = completion.Message.Text ?? string.Empty;

        // 4. Update Session
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

        return new ReGenerateCodeResult(resultId.Value, refinedCodeText);
    }
}
