using AiOrchestration.ValueObjects;
using CodeDebug.Data;
using CodeDebug.Models;
using CodeDebug.ValueObjects;
using MediatR;
using Microsoft.Extensions.AI;
using Ardalis.GuardClauses;
using AiOrchestration.Services;
using System.Runtime.CompilerServices;
using System.Text;

namespace CodeDebug.Features.StreamAnalyzeCode.V1;

internal class StreamAnalyzeCodeHandler : IStreamRequestHandler<StreamAnalyzeCodeCommand, string>
{
    private readonly CodeDebugDbContext _dbContext;
    private readonly IAiOrchestrator _chatClient;

    public StreamAnalyzeCodeHandler(CodeDebugDbContext dbContext, IAiOrchestrator chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async IAsyncEnumerable<string> Handle(StreamAnalyzeCodeCommand request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(request.Code, nameof(request.Code));

        var prompt = $"Analyze the following {request.Language} code for bugs. Provide a detailed report.\n\nCode:\n{request.Code}";
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "You are an expert debugger. Provide a detailed analysis report."),
            new ChatMessage(ChatRole.User, prompt)
        };

        var fullReportBuilder = new StringBuilder();
        int tokenEstimate = 0;

        // Use chatClient to get the best client
        var chatClient = await _chatClient.GetClientAsync(cancellationToken: cancellationToken);
        var response = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        foreach (var update in response.Messages)
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                fullReportBuilder.Append(update.Text);
                tokenEstimate++;
                yield return update.Text;
            }
        }

        // Persist
        await PersistReportAsync(request, fullReportBuilder.ToString(), tokenEstimate, cancellationToken);
    }

    private async Task PersistReportAsync(StreamAnalyzeCodeCommand request, string fullReport, int tokenUsage, CancellationToken cancellationToken)
    {
        try
        {
            var sessionId = CodeDebugId.Of(Guid.NewGuid());
            var userId = UserId.Of(Guid.NewGuid());
            var modelId = ModelId.Of(_chatClient.Metadata.ModelId ?? "debug-stream-model");
            var config = CodeDebugConfiguration.Of("streaming");

            var session = CodeDebugSession.Create(sessionId, userId, modelId, config);

            var reportId = CodeDebugReportId.Of(Guid.NewGuid());
            var codeVo = SourceCode.Of(request.Code);
            var summaryVo = DebugSummary.Of(fullReport);
            var issueCountVo = IssueCount.Of(1); // Placeholder or parse
            var tokenCountVo = TokenCount.Of(tokenUsage);
            var costVo = CostEstimate.Of(0);

            var report = CodeDebugReport.Create(reportId, codeVo, request.Language, summaryVo, issueCountVo, tokenCountVo, costVo);
            session.AddReport(report);

            _dbContext.Sessions.Add(session);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // Log error
        }
    }
}

