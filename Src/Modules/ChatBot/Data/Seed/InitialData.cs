namespace ChatBot.Data.Seed;

using AI.Common.Core;
using global::ChatBot.Models;
using global::ChatBot.ValueObjects;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<ChatModel> Chats { get; } = new()
    {
        ChatModel.Create(SessionId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a")), UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), "Getting started with AI", "gpt-4"),
        ChatModel.Create(SessionId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b")), UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), "Cooking Recipes", "gpt-3.5-turbo")
    };

    static InitialData()
    {
        var sessionId1 = SessionId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a"));
        Chats[0].AddMessage(MessageModel.Create(MessageId.Of(Guid.NewGuid()), sessionId1, MessageSender.Of(ChatBot.Enums.MessageSender.User.ToString()), MessageContent.Of("Hello, what is AI?"), TokenUsed.Of(10)));
        Chats[0].AddMessage(MessageModel.Create(MessageId.Of(Guid.NewGuid()), sessionId1, MessageSender.Of(ChatBot.Enums.MessageSender.AI.ToString()), MessageContent.Of("AI stands for Artificial Intelligence..."), TokenUsed.Of(50)));

        var sessionId2 = SessionId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b"));
        Chats[1].AddMessage(MessageModel.Create(MessageId.Of(Guid.NewGuid()), sessionId2, MessageSender.Of(ChatBot.Enums.MessageSender.User.ToString()), MessageContent.Of("How to make a pizza?"), TokenUsed.Of(12)));
        Chats[1].AddMessage(MessageModel.Create(MessageId.Of(Guid.NewGuid()), sessionId2, MessageSender.Of(ChatBot.Enums.MessageSender.AI.ToString()), MessageContent.Of("To make a pizza, you need dough, sauce..."), TokenUsed.Of(100)));
    }
}