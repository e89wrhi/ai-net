namespace ChatBot.Data.Seed;

using AiOrchestration.ValueObjects;
using ChatBot.Enums;
using ChatBot.Models;
using ChatBot.ValueObjects;

public static class InitialData
{
    public static List<ChatSession> Chats { get; } = new();

    static InitialData()
    {
        // Create default configuration for seed data
        var defaultConfig = new ChatConfiguration(
            Temperature.Of(0.7f),
            TokenCount.Of(4096), 
            SystemPrompt.Of("You are a helpful AI assistant.")
        );

        // Create chat sessions
        var chat1 = ChatSession.Create(
            SessionId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            "Getting started with AI", 
            ModelId.Of("gpt-4"),
            defaultConfig);

        var chat2 = ChatSession.Create(
            SessionId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            "Cooking Recipes", 
            ModelId.Of("gpt-3.5-turbo"),
            defaultConfig);

        // Add messages to chat 1
        chat1.AddMessage(ChatMessage.Create(
            MessageId.Of(Guid.NewGuid()), 
            chat1.Id,
            MessageSender.User, 
            MessageContent.Of("Hello, what is AI?"), 
            TokenCount.Of(10),
            CostEstimate.Of(0),
            1));

        chat1.AddMessage(ChatMessage.Create(
            MessageId.Of(Guid.NewGuid()), 
            chat1.Id,
            MessageSender.Assistant, 
            MessageContent.Of("AI stands for Artificial Intelligence..."), 
            TokenCount.Of(50),
            CostEstimate.Of(0.001m),
            2));

        // Add messages to chat 2
        chat2.AddMessage(ChatMessage.Create(
            MessageId.Of(Guid.NewGuid()), 
            chat2.Id,
            MessageSender.User, 
            MessageContent.Of("How to make a pizza?"), 
            TokenCount.Of(12),
            CostEstimate.Of(0),
            1));

        chat2.AddMessage(ChatMessage.Create(
            MessageId.Of(Guid.NewGuid()), 
            chat2.Id,
            MessageSender.Assistant, 
            MessageContent.Of("To make a pizza, you need dough, sauce..."), 
            TokenCount.Of(100),
            CostEstimate.Of(0.002m),
            2));

        Chats.Add(chat1);
        Chats.Add(chat2);
    }
}