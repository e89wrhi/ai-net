using AiOrchestration.ValueObjects;
using ChatBot.Data;
using ChatBot.Enums;
using ChatBot.Exceptions;
using ChatBot.Models;
using Ardalis.GuardClauses;
using AiOrchestration.Services;
using ChatBot.ValueObjects;
using MediatR;
using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;
using System.Text;

namespace ChatBot.Features.StreamResponse.V1;


internal class StreamAiResponseHandler : IStreamRequestHandler<StreamAiResponseCommand, string>
{
    private readonly ChatDbContext _dbContext;
    private readonly IAiOrchestrator _chatClient;

    public StreamAiResponseHandler(ChatDbContext dbContext, IAiOrchestrator chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async IAsyncEnumerable<string> Handle(StreamAiResponseCommand request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        // Load Session (Basic load)
        var chat = await _dbContext.Chats.FindAsync(new object[] { SessionId.Of(request.SessionId) }, cancellationToken);
        if (chat == null) throw new ChatNotFoundException(request.SessionId);

        // NOTE: Context loading logic is simplified here same as GenerateAiResponse.
        // Ideally we fetch recent messages.

        var chatMessages = new List<Microsoft.Extensions.AI.ChatMessage>
        {
             new(ChatRole.System, $"You are a helpful assistant for session: {chat.Title}")
             // Add history here
        };

        var fullResponseBuilder = new StringBuilder();
        int tokenEstimate = 0;

        // Use chatClient to get the best client
        var chatClient = await _chatClient.GetClientAsync(cancellationToken: cancellationToken);
        var response = await chatClient.GetResponseAsync(chatMessages, cancellationToken: cancellationToken);
        foreach (var update in response.Messages)
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                fullResponseBuilder.Append(update.Text);
                tokenEstimate++;
                yield return update.Text;
            }
        }

        // Persist interaction
        await PersistMessageAsync(chat, fullResponseBuilder.ToString(), tokenEstimate, cancellationToken);
    }

    private async Task PersistMessageAsync(ChatSession chat, string content, int tokenUsage, CancellationToken cancellationToken)
    {
        try
        {
            var messageId = MessageId.Of(Guid.NewGuid());
            // Need to know latest sequence. 
            // Since we didn't lock or load full collection, this is optimistic.
            // But simple increment is better than nothing.
            int sequence = 999;

            var aiMessage = ChatMessage.Create(
                messageId,
                MessageSender.Assistant,
                MessageContent.Of(content),
                TokenCount.Of(tokenUsage),
                CostEstimate.Of(0),
                sequence
            );

            chat.AddMessage(aiMessage);
            // Re-attach if needed or just save if tracked
            if (_dbContext.Entry(chat).State == Microsoft.EntityFrameworkCore.EntityState.Detached)
            {
                _dbContext.Chats.Attach(chat);
                // Mark modified if needed, but AddMessage modifies state via helper usually
            }
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // Log error
        }
    }
}
