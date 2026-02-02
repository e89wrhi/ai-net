namespace SpeechToText.Data.Seed;

using AI.Common.Core;
using global::SpeechToText.Models;
using global::SpeechToText.ValueObjects;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<SpeechToTextSession> SpeechToTexts { get; } = new()
    {
        SpeechToTextModel.Create(SessionId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a")), UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), "Getting started with AI", "gpt-4"),
        SpeechToTextModel.Create(SessionId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b")), UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), "Cooking Recipes", "gpt-3.5-turbo")
    };

    static InitialData()
    {
        var sessionId1 = SessionId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a"));
        SpeechToTexts[0].AddSpeechToText(SpeechToTextModel.Create(SpeechToTextId.Of(Guid.NewGuid()), sessionId1, ValueObjects.SpeechToTextConfiguration.Of(SpeechToText.Enums.SpeechToTextStatus.User.ToString()), ValueObjects.Transcript.Of("Hello, what is AI?"), ValueObjects.AudioSource.Of(10)));
        SpeechToTexts[0].AddSpeechToText(SpeechToTextModel.Create(SpeechToTextId.Of(Guid.NewGuid()), sessionId1, ValueObjects.SpeechToTextConfiguration.Of(SpeechToText.Enums.SpeechToTextStatus.System.ToString()), ValueObjects.Transcript.Of("AI stands for Artificial Intelligence..."), ValueObjects.AudioSource.Of(50)));

        var sessionId2 = SessionId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b"));
        SpeechToTexts[1].AddSpeechToText(SpeechToTextModel.Create(SpeechToTextId.Of(Guid.NewGuid()), sessionId2, ValueObjects.SpeechToTextConfiguration.Of(SpeechToText.Enums.SpeechToTextStatus.User.ToString()), ValueObjects.Transcript.Of("How to make a pizza?"), ValueObjects.AudioSource.Of(12)));
        SpeechToTexts[1].AddSpeechToText(SpeechToTextModel.Create(SpeechToTextId.Of(Guid.NewGuid()), sessionId2, ValueObjects.SpeechToTextConfiguration.Of(SpeechToText.Enums.SpeechToTextStatus.System.ToString()), ValueObjects.Transcript.Of("To make a pizza, you need dough, sauce..."), ValueObjects.AudioSource.Of(100)));
    }
}