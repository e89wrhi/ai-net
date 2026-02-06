namespace LearningAssistant.Data.Seed;

using AiOrchestration.ValueObjects;
using LearningAssistant.Models;
using LearningAssistant.ValueObjects;
using LearningAssistant.Enums;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<LearningSession> Profiles { get; } = new()
    {
        LearningSession.Create(
            LearningId.Of(Guid.Parse("ba8fad5b-d9cb-469f-a165-70867728950f")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            ModelId.Of("gpt-4"),
            new LearningConfiguration(global::LearningAssistant.Enums.LearningMode.Interactive, global::LearningAssistant.Enums.DifficultyLevel.Medium))
    };

    static InitialData()
    {
        Profiles[0].AddActivity(LearningActivity.Create(
            ActivityId.Of(Guid.NewGuid()), 
            LearningTopic.Of("Introduction to AI"),
            LearningContent.Of("AI fundamentals and concepts..."),
            LearningOutcome.Of("Understood basic concepts"),
            TokenCount.Of(300),
            CostEstimate.Of(0.005m)));
    }
}