namespace Sentiment.Data.Seed;

using AI.Common.Core;
using global::Sentiment.Models;
using global::Sentiment.ValueObjects;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<SentimentModel> Sentiments { get; } = new()
    {
        SentimentModel.Create(SessionId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a")), UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), "Getting started with AI", "gpt-4"),
        SentimentModel.Create(SessionId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b")), UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), "Cooking Recipes", "gpt-3.5-turbo")
    };

    static InitialData()
    {
        var sessionId1 = SessionId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a"));
        Sentiments[0].AddSentiment(SentimentModel.Create(SentimentResultId.Of(Guid.NewGuid()), sessionId1, ValueObjects.TextSentimentConfiguration.Of(SentimentText.Enums.SentimentSender.User.ToString()), ValueObjects.SentimentScore.Of("Hello, what is AI?"), TokenUsed.Of(10)));
        Sentiments[0].AddSentiment(SentimentModel.Create(SentimentResultId.Of(Guid.NewGuid()), sessionId1, ValueObjects.TextSentimentConfiguration.Of(SentimentText.Enums.SentimentSender.System.ToString()), ValueObjects.SentimentScore.Of("AI stands for Artificial Intelligence..."), TokenUsed.Of(50)));

        var sessionId2 = SessionId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b"));
        Sentiments[1].AddSentiment(SentimentModel.Create(SentimentResultId.Of(Guid.NewGuid()), sessionId2, ValueObjects.TextSentimentConfiguration.Of(SentimentText.Enums.SentimentSender.User.ToString()), ValueObjects.SentimentScore.Of("How to make a pizza?"), TokenUsed.Of(12)));
        Sentiments[1].AddSentiment(SentimentModel.Create(SentimentResultId.Of(Guid.NewGuid()), sessionId2, ValueObjects.TextSentimentConfiguration.Of(SentimentText.Enums.SentimentSender.System.ToString()), ValueObjects.SentimentScore.Of("To make a pizza, you need dough, sauce..."), TokenUsed.Of(100)));
    }
}