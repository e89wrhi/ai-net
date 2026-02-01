using System.Runtime.CompilerServices;
using System.Text;
using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using ChatBot.Data;
using ChatBot.Enums;
using ChatBot.Exceptions;
using ChatBot.Models;
using ChatBot.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace ChatBot.Features.StreamAiResponse.V1;

public record StreamAiResponseCommand(Guid SessionId) : IStreamRequest<string>;

public class StreamAiResponseEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/chat/stream-response",
                (StreamAiResponseRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    return mediator.CreateStream(new StreamAiResponseCommand(request.SessionId), cancellationToken);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StreamAiResponse")
            .WithApiVersionSet(builder.NewApiVersionSet("Chat").Build())
            .Produces<IAsyncEnumerable<string>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Stream AI Response")
            .WithDescription("Streams the AI response for the given chat session.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public record StreamAiResponseRequestDto(Guid SessionId);

internal class StreamAiResponseHandler : IStreamRequestHandler<StreamAiResponseCommand, string>
{
    private readonly ChatDbContext _dbContext;
    private readonly IChatClient _chatClient;

    public StreamAiResponseHandler(ChatDbContext dbContext, IChatClient chatClient)
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

        await foreach (var update in _chatClient.CompleteStreamingAsync(chatMessages, cancellationToken: cancellationToken))
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
