namespace Sentiment.Data.Seed;

using AI.Common.Core;
using AiOrchestration.ValueObjects;
using global::Sentiment.Models;
using global::Sentiment.ValueObjects;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<TextSentimentSession> Sentiments { get; } = new()
    {
        TextSentimentSession.Create(
            SentimentId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            ModelId.Of("gpt-4"),
            new TextSentimentConfiguration(global::Sentiment.Enums.SentimentDetailLevel.Standard, LanguageCode.Of("en"))),
        TextSentimentSession.Create(
            SentimentId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            ModelId.Of("gpt-3.5-turbo"),
            new TextSentimentConfiguration(global::Sentiment.Enums.SentimentDetailLevel.Detailed, LanguageCode.Of("en")))
    };

    static InitialData()
    {
        Sentiments[0].AddResult(TextSentimentResult.Create(
            SentimentResultId.Of(Guid.NewGuid()), 
            "I absolutely love this product! It's amazing!",
            global::Sentiment.ValueObjects.SentimentText.Of("Positive"),
            SentimentScore.Of(0.95f),
            TokenCount.Of(50),
            AiOrchestration.ValueObjects.CostEstimate.Of(0.001m)));
            
        Sentiments[1].AddResult(TextSentimentResult.Create(
            SentimentResultId.Of(Guid.NewGuid()), 
            "This is terrible. I'm very disappointed.",
            global::Sentiment.ValueObjects.SentimentText.Of("Negative"),
            SentimentScore.Of(0.15f),
            TokenCount.Of(45),
            AiOrchestration.ValueObjects.CostEstimate.Of(0.0009m)));
    }
}