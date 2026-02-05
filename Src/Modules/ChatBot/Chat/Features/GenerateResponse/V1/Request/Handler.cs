using AI.Common.Core;
using AiOrchestration.Models;
using AiOrchestration.Services;
using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using ChatBot.Data;
using ChatBot.Enums;
using ChatBot.Exceptions;
using ChatBot.Models;
using ChatBot.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;

namespace ChatBot.Features.GenerateResponse.V1;

internal class GenerateAiResponseHandler : ICommandHandler<GenerateAiResponseCommand, GenerateAiResponseCommandResult>
{
    private readonly ChatDbContext _dbContext;
    private readonly IAiOrchestrator _orchestrator;
    private readonly IAiModelService _modelService;

    public GenerateAiResponseHandler(ChatDbContext dbContext, IAiModelService modelService, IAiOrchestrator orchestrator)
    {
        _dbContext = dbContext;
        _modelService = modelService;
        _orchestrator = orchestrator;
    }

    public async Task<GenerateAiResponseCommandResult> Handle(GenerateAiResponseCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        // Load the chat session with messages for context
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

        // Use orchestrator to get the client based on requested model criteria
        var criteria = new ModelCriteria { ModelId = request.ModelId };
        var chatClient = await _orchestrator.GetClientAsync(criteria, cancellationToken);

        // Get actual model info from client metadata
        var clientMetadata = chatClient.GetService(typeof(ChatClientMetadata)) as ChatClientMetadata;
        var modelIdStr = clientMetadata?.DefaultModelId ?? request.ModelId;
        var providerName = clientMetadata?.ProviderName ?? "Unknown";

        // Call AI to generate response
        var completion = await chatClient.GetResponseAsync(chatMessages, cancellationToken: cancellationToken);
        var responseContent = completion.Messages.LastOrDefault()?.Text ?? "I'm sorry, I couldn't generate a response.";

        // Calculate token usage and cost
        var tokenUsage = completion.Usage?.TotalTokenCount ?? 0;
        var modelId = ModelId.Of(modelIdStr);
        var costPerToken = _modelService.GetCostPerToken(modelId);
        var costValue = (decimal)tokenUsage * costPerToken;

        // Create and add the AI response message to the chat
        var messageId = MessageId.Of(Guid.NewGuid());
        var sequence = chat.Messages.Count + 1;

        var aiMessage = Models.ChatMessage.Create(
            messageId,
            chat.Id,
            MessageSender.Assistant,
            MessageContent.Of(responseContent),
            TokenCount.Of(tokenUsage),
            CostEstimate.Of(costValue),
            sequence
        );

        chat.AddMessage(aiMessage);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new GenerateAiResponseCommandResult(messageId.Value, responseContent, modelIdStr, providerName);
    }
}
