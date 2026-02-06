namespace Summary.Data.Seed;

using AI.Common.Core;
using AiOrchestration.ValueObjects;
using global::Summary.Models;
using global::Summary.ValueObjects;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<TextSummarySession> Summarys { get; } = new()
    {
        TextSummarySession.Create(
            SummaryId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            ModelId.Of("gpt-4"),
            new TextSummaryConfiguration(global::Summary.Enums.SummaryDetailLevel.Standard, LanguageCode.Of("en"))),
        TextSummarySession.Create(
            SummaryId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            ModelId.Of("gpt-3.5-turbo"),
            new TextSummaryConfiguration(global::Summary.Enums.SummaryDetailLevel.Detailed, LanguageCode.Of("en")))
    };

    static InitialData()
    {
        Summarys[0].AddResult(TextSummaryResult.Create(
            SummaryResultId.Of(Guid.NewGuid()), 
            "The quick brown fox jumps over the lazy dog.",
            SummaryText.Of("A fox jumps over a dog."),
            TokenCount.Of(50),
            AiOrchestration.ValueObjects.CostEstimate.Of(0.001m)));
            
        Summarys[1].AddResult(TextSummaryResult.Create(
            SummaryResultId.Of(Guid.NewGuid()), 
            "Artificial Intelligence is the simulation of human intelligence processes by machines.",
            SummaryText.Of("AI simulates human intelligence."),
            TokenCount.Of(60),
            AiOrchestration.ValueObjects.CostEstimate.Of(0.0012m)));
    }
}