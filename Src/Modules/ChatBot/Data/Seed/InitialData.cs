namespace ChatBot.Data.Seed;

using AI.Common.Core;
using global::ChatBot.Models;
using global::ChatBot.ValueObjects;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<ChatSession> Chats { get; } = new()
    {
        ChatSession.Create(SessionId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a")), UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), "Getting started with AI", "gpt-4"),
        ChatSession.Create(SessionId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b")), UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), "Cooking Recipes", "gpt-3.5-turbo")
    };

    static InitialData()
    {
        var sessionId1 = SessionId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a"));
        Chats[0].AddMessage(ChatMessage.Create(MessageId.Of(Guid.NewGuid()), sessionId1, ValueObjects.MessageContent.Of(ChatBot.Enums.MessageSender.User.ToString()), Models.MessageContent.Of("Hello, what is AI?"), ValueObjects.MaxTokens.Of(10)));
        Chats[0].AddMessage(ChatMessage.Create(MessageId.Of(Guid.NewGuid()), sessionId1, ValueObjects.MessageContent.Of(ChatBot.Enums.MessageSender.System.ToString()), Models.MessageContent.Of("AI stands for Artificial Intelligence..."), ValueObjects.MaxTokens.Of(50)));

        var sessionId2 = SessionId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b"));
        Chats[1].AddMessage(ChatMessage.Create(MessageId.Of(Guid.NewGuid()), sessionId2, ValueObjects.MessageContent.Of(ChatBot.Enums.MessageSender.User.ToString()), Models.MessageContent.Of("How to make a pizza?"), ValueObjects.MaxTokens.Of(12)));
        Chats[1].AddMessage(ChatMessage.Create(MessageId.Of(Guid.NewGuid()), sessionId2, ValueObjects.MessageContent.Of(ChatBot.Enums.MessageSender.System.ToString()), Models.MessageContent.Of("To make a pizza, you need dough, sauce..."), ValueObjects.MaxTokens.Of(100)));
    }
}