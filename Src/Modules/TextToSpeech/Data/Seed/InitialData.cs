namespace TextToSpeech.Data.Seed;

using AI.Common.Core;
using global::TextToSpeech.Models;
using global::TextToSpeech.ValueObjects;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<TextToSpeechModel> TextToSpeechs { get; } = new()
    {
        TextToSpeechModel.Create(SessionId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a")), UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), "Getting started with AI", "gpt-4"),
        TextToSpeechModel.Create(SessionId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b")), UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), "Cooking Recipes", "gpt-3.5-turbo")
    };

    static InitialData()
    {
        var sessionId1 = SessionId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a"));
        TextToSpeechs[0].AddTextToSpeech(TextToSpeechModel.Create(TextToSpeechId.Of(Guid.NewGuid()), sessionId1, ValueObjects.TextToSpeechConfiguration.Of(TextToSpeech.Enums.TextToSpeechStatus.User.ToString()), ValueObjects.SynthesizedSpeech.Of("Hello, what is AI?"), TokenUsed.Of(10)));
        TextToSpeechs[0].AddTextToSpeech(TextToSpeechModel.Create(TextToSpeechId.Of(Guid.NewGuid()), sessionId1, ValueObjects.TextToSpeechConfiguration.Of(TextToSpeech.Enums.TextToSpeechStatus.System.ToString()), ValueObjects.SynthesizedSpeech.Of("AI stands for Artificial Intelligence..."), TokenUsed.Of(50)));

        var sessionId2 = SessionId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b"));
        TextToSpeechs[1].AddTextToSpeech(TextToSpeechModel.Create(TextToSpeechId.Of(Guid.NewGuid()), sessionId2, ValueObjects.TextToSpeechConfiguration.Of(TextToSpeech.Enums.TextToSpeechStatus.User.ToString()), ValueObjects.SynthesizedSpeech.Of("How to make a pizza?"), TokenUsed.Of(12)));
        TextToSpeechs[1].AddTextToSpeech(TextToSpeechModel.Create(TextToSpeechId.Of(Guid.NewGuid()), sessionId2, ValueObjects.TextToSpeechConfiguration.Of(TextToSpeech.Enums.TextToSpeechStatus.System.ToString()), ValueObjects.SynthesizedSpeech.Of("To make a pizza, you need dough, sauce..."), TokenUsed.Of(100)));
    }
}