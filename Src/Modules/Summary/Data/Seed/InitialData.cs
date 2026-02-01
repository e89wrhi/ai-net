namespace Summary.Data.Seed;

using AI.Common.Core;
using global::Summary.Models;
using global::Summary.ValueObjects;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<SummaryModel> Summarys { get; } = new()
    {
        SummaryModel.Create(SessionId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a")), UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), "Getting started with AI", "gpt-4"),
        SummaryModel.Create(SessionId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b")), UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), "Cooking Recipes", "gpt-3.5-turbo")
    };

    static InitialData()
    {
        var sessionId1 = SessionId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a"));
        Summarys[0].AddSummary(SummaryModel.Create(SummaryId.Of(Guid.NewGuid()), sessionId1, SummaryConfiguration.Of(Summary.Enums.TextSummaryStatus.User.ToString()), ValueObjects.SummaryText.Of("Hello, what is AI?"), TokenUsed.Of(10)));
        Summarys[0].AddSummary(SummaryModel.Create(SummaryId.Of(Guid.NewGuid()), sessionId1, SummaryConfiguration.Of(Summary.Enums.TextSummaryStatus.System.ToString()), ValueObjects.SummaryText.Of("AI stands for Artificial Intelligence..."), TokenUsed.Of(50)));

        var sessionId2 = SessionId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b"));
        Summarys[1].AddSummary(SummaryModel.Create(SummaryId.Of(Guid.NewGuid()), sessionId2, SummaryConfiguration.Of(Summary.Enums.TextSummaryStatus.User.ToString()), ValueObjects.SummaryText.Of("How to make a pizza?"), TokenUsed.Of(12)));
        Summarys[1].AddSummary(SummaryModel.Create(SummaryId.Of(Guid.NewGuid()), sessionId2, SummaryConfiguration.Of(Summary.Enums.TextSummaryStatus.System.ToString()), ValueObjects.SummaryText.Of("To make a pizza, you need dough, sauce..."), TokenUsed.Of(100)));
    }
}