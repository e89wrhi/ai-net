using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using ChatBot.Data;
using ChatBot.Enums;
using ChatBot.Exceptions;
using ChatBot.Models;
using ChatBot.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;
using System.Text;

namespace ChatBot.Features.StreamResponse.V1;

internal class StreamAiResponseHandler : IStreamRequestHandler<StreamAiResponseCommand, string>
{
    private readonly ChatDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IAiModelService _modelService;

    public StreamAiResponseHandler(ChatDbContext dbContext, IAiOrchestrator orchestrator, IAiModelService modelService)
    {
        _dbContext = dbContext;
        _orchestrator = orchestrator;
        _modelService = modelService;
    }

    public async IAsyncEnumerable<string> Handle(StreamAiResponseCommand request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        // Load chat session with messages for context
        var chat = await _dbContext.Chats
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == SessionId.Of(request.SessionId), cancellationToken);

        if (chat == null) 
            throw new ChatNotFoundException(request.SessionId);

        // Build conversation history for AI context
        var chatMessages = new List<Microsoft.Extensions.AI.ChatMessage>();
        
        // Add system prompt if configured
        if (!string.IsNullOrEmpty(chat.Configuration.SystemPrompt.Value))
        {
            chatMessages.Add(new Microsoft.Extensions.AI.ChatMessage(ChatRole.System, chat.Configuration.SystemPrompt.Value));
        }

        // Add message history for context
        foreach (var msg in chat.Messages.OrderBy(m => m.Sequence))
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

        // Get AI client from orchestrator
        var criteria = new ModelCriteria { ModelId = chat.AiModelId.Value };
        var chatClient = await _orchestrator.GetClientAsync(criteria, cancellationToken);

        // Get model metadata
        var clientMetadata = chatClient.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata;
        var modelIdStr = clientMetadata?.DefaultModelId ?? chat.AiModelId.Value;

        var fullResponseBuilder = new StringBuilder();
        int totalTokens = 0;
        int tokensTracked = 0;

        // Stream the AI response
        await foreach (var update in chatClient.GetStreamingResponseAsync(chatMessages, cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                fullResponseBuilder.Append(update.Text);
                tokensTracked++;
                yield return update.Text;
            }
        }

        // After streaming completes, try to get final completion for accurate token count
        // For now, estimate based on content
        totalTokens = fullResponseBuilder.Length / 4; // Rough estimate

        // Calculate cost
        var modelId = ModelId.Of(modelIdStr);
        var costPerToken = _modelService.GetCostPerToken(modelId);
        var costValue = (decimal)totalTokens * costPerToken;

        // Persist the complete message
        await PersistMessageAsync(chat, fullResponseBuilder.ToString(), totalTokens, costValue, cancellationToken);
    }

    private async Task PersistMessageAsync(ChatSession chat, string content, int tokenUsage, decimal cost, CancellationToken cancellationToken)
    {
        try
        {
            var messageId = MessageId.Of(Guid.NewGuid());
            var sequence = chat.Messages.Count + 1;

            var aiMessage = Models.ChatMessage.Create(
                messageId,
                chat.Id,
                MessageSender.Assistant,
                MessageContent.Of(content),
                TokenCount.Of(tokenUsage),
                CostEstimate.Of(cost),
                sequence
            );

            chat.AddMessage(aiMessage);
            
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // Log error - in production use proper logging
            Console.WriteLine($"Error persisting streamed message: {ex.Message}");
        }
    }
}
