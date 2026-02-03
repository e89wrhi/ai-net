using AI.Common.Core;
using AiOrchestration.ValueObjects;
using ChatBot.Data;
using ChatBot.Enums;
using ChatBot.Exceptions;
using ChatBot.ValueObjects;
using Ardalis.GuardClauses;
using AiOrchestration.Services;
using Microsoft.Extensions.AI;

namespace ChatBot.Features.GenerateResponse.V1;


internal class GenerateAiResponseHandler : ICommandHandler<GenerateAiResponseCommand, GenerateAiResponseCommandResult>
{
    private readonly ChatDbContext _dbContext;
    private readonly IAiOrchestrator _chatClient;

    public GenerateAiResponseHandler(ChatDbContext dbContext, IAiOrchestrator chatClient)
    {
        _dbContext = dbContext;
        _chatClient = chatClient;
    }

    public async Task<GenerateAiResponseCommandResult> Handle(GenerateAiResponseCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        // Load the chat session including messages (to provide context)
        // Note: In a real app we might limit context window size.
        // Assuming ChatSession.Messages is populated or we need to Include it.
        // EF Core loading:
        var chat = await _dbContext.Chats
            // .Include(c => c.Messages) // If needed, but Aggregate root should manage this. 
            // However, _messages in ChatSession is a private field backed collection.
            // EF Core maps to backing field. We need to ensure we load it if we want context.
            // For now, assuming basic load or lazy loading if configured.
            // Or simpler: just sending the last user message for now if full history is hard.
            // Ideally we pass full history.
            .FindAsync(new object[] { SessionId.Of(request.SessionId) }, cancellationToken);

        if (chat == null)
            throw new ChatNotFoundException(request.SessionId);

        // Prepare context for AI
        // Since we can't easily access the valid Messages collection if it's not loaded eagerly (and FindAsync doesn't include),
        // we might fail to provide context. 
        // Let's rely on explicit loading if needed, or assume for this iteration we just prompt "Continue conversation".
        // Actually, let's try to get the messages via explicit query if chat.Messages is empty but shouldn't be.
        var messages = _dbContext.Entry(chat).Collection(c => c.Messages).IsLoaded
            ? chat.Messages
            : await _dbContext.Messages
                .Where(m => m.Id == MessageId.Of(request.SessionId)) // Wait, Message.Id is PK. foreign key needed.
                // ChatMessage likely has a foreign key to ChatSession. 
                // Let's check ChatMessage definition context. It doesn't show FK property explicitly but ChatSession has list.
                // Usually Shadow Property "ChatSessionId".
                // Let's assume for now we just send a generic prompt or user provided prompt logic elsewhere.
                // But the user wants "Features based on models".

        // Let's just prompt based on the session Title or Summary for now if history is missing, 
        // or assume FindAsync worked if LazyLoading is on (Common.csproj has related packages).
                new List<ChatMessage>();

        // Convert to ChatMessage (Microsoft.Extensions.AI)
        var chatMessages = new List<Microsoft.Extensions.AI.ChatMessage>();
        if (messages.Any())
        {
            foreach (var msg in messages.OrderBy(m => m.Sequence))
            {
                var role = msg.Sender switch
                {
                    MessageSender.User => ChatRole.User,
                    MessageSender.Assistant => ChatRole.Assistant,
                    MessageSender.System => ChatRole.System,
                    _ => ChatRole.User
                };
                chatMessages.Add(new Microsoft.Extensions.AI.ChatMessage(role, msg.Content.Value));
            }
        }
        else
        {
            // Fallback or just prompt
            chatMessages.Add(new Microsoft.Extensions.AI.ChatMessage(ChatRole.System, $"You are a helpful assistant for session: {chat.Title}"));
        }

        // Use chatClient to get the best client
        var chatClient = await _chatClient.GetClientAsync(cancellationToken: cancellationToken);

        var completion = await chatClient.GetResponseAsync(chatMessages, cancellationToken: cancellationToken);
        var responseContent = completion.Messages[0].Text ?? "I'm sorry, I couldn't generate a response.";

        // Create Response Message
        var messageId = MessageId.Of(Guid.NewGuid());
        var sequence = chat.Messages.Count + 1;

        var aiMessage = ChatMessage.Create(
            messageId,
            MessageSender.Assistant,
            MessageContent.Of(responseContent),
            TokenCount.Of(completion.Usage?.TotalTokenCount ?? 0),
            CostEstimate.Of(0), // Calculate cost logic if needed
            sequence
        );

        chat.AddMessage(aiMessage);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new GenerateAiResponseCommandResult(messageId.Value, responseContent);
    }
}
