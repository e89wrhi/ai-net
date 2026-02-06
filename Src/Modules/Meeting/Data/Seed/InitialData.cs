namespace Meeting.Data.Seed;

using AI.Common.Core;
using AiOrchestration.ValueObjects;
using global::Meeting.Models;
using global::Meeting.ValueObjects;
using global::Meeting.Enums;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<MeetingAnalysisSession> Meetings { get; } = new()
    {
        MeetingAnalysisSession.Create(
            MeetingId.Of(Guid.Parse("ca8fad5b-d9cb-469f-a165-708677289510")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            ModelId.Of("gpt-4"),
            new MeetingAnalysisConfiguration(true, true, LanguageCode.Of("en")))
    };

    static InitialData()
    {
        Meetings[0].AddTranscript(MeetingTranscript.Create(
            TranscriptId.Of(Guid.NewGuid()), 
            "Today we discussed the project timeline...",
            TranscriptSummary.Of("Project timeline sync"),
            TokenCount.Of(500),
            CostEstimate.Of(0.01m)));
    }
}