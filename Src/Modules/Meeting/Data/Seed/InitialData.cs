namespace Meeting.Data.Seed;

using AI.Common.Core;
using global::Meeting.Models;
using global::Meeting.ValueObjects;
using global::Meeting.Enums;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<MeetingModel> Meetings { get; } = new()
    {
        MeetingModel.Create(MeetingId.Of(Guid.Parse("ca8fad5b-d9cb-469f-a165-708677289510")), "0f8fad5b-d9cb-469f-a165-70867728950e", Title.Of("Weekly Sync"), AudioSource.Of("recordings/weekly_sync.mp3"))
    };

    static InitialData()
    {
        Meetings[0].CompleteTranscription("Today we discussed the project timeline...", TranscriptLanguage.EN, 0.99, "Project timeline sync");
        Meetings[0].AddActionItem(new ActionItem(Guid.NewGuid(), Meetings[0].Id,
            "Update the roadmap by Friday"));
    }
}