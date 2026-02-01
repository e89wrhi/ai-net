using System.Runtime.CompilerServices;
using System.Text;
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

namespace CodeGen.Features.StreamGenerateCode.V1;

public record StreamGenerateCodeCommand(string Prompt, string Language) : IStreamRequest<string>;

public class StreamGenerateCodeEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/codegen/generate-stream",
                (StreamGenerateCodeRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    return mediator.CreateStream(new StreamGenerateCodeCommand(request.Prompt, request.Language), cancellationToken);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StreamGenerateCode")
            .WithApiVersionSet(builder.NewApiVersionSet("CodeGen").Build())
            .Produces<IAsyncEnumerable<string>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Stream Generate Code")
            .WithDescription("Streams the generated code using AI.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public record StreamGenerateCodeRequestDto(string Prompt, string Language);

internal class StreamGenerateCodeHandler : IStreamRequestHandler<StreamGenerateCodeCommand, string>
{
    private readonly CodeGenDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public StreamGenerateCodeHandler(CodeGenDbContext dbContext, IChatClient chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
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

        await foreach (var update in _chatClient.CompleteStreamingAsync(messages, cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                fullCodeBuilder.Append(update.Text);
                tokenEstimate++;
                yield return update.Text;
            }
        }

        // Persist
        await PersistResultAsync(request, fullCodeBuilder.ToString(), tokenEstimate, cancellationToken);
    }

    private async Task PersistResultAsync(StreamGenerateCodeCommand request, string fullCode, int tokenUsage, CancellationToken cancellationToken)
    {
        try 
        {
            var sessionId = CodeGenId.Of(Guid.NewGuid());
            var userId = UserId.Of(Guid.NewGuid());
            var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "codegen-stream-model");
            var config = CodeGenerationConfiguration.Of("streaming");

            var session = CodeGenerationSession.Create(sessionId, userId, modelId, config);

            var resultId = CodeGenResultId.Of(Guid.NewGuid());
            var promptVo = CodeGenerationPrompt.Of(request.Prompt);
            var codeVo = GeneratedCode.Of(fullCode);
            var languageVo = ProgrammingLanguage.Of(request.Language);
            var tokenCountVo = TokenCount.Of(tokenUsage);
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
        }
        catch
        {
            // Log error
        }
    }
}
