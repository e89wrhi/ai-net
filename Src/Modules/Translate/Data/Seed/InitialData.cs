namespace Translate.Data.Seed;

using AI.Common.Core;
using global::Translate.Models;
using global::Translate.ValueObjects;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<TranslationSession> Translates { get; } = new()
    {
        TranslateModel.Create(SessionId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a")), UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), "Getting started with AI", "gpt-4"),
        TranslateModel.Create(SessionId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b")), UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), "Cooking Recipes", "gpt-3.5-turbo")
    };

    static InitialData()
    {
        var sessionId1 = SessionId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a"));
        Translates[0].AddTranslate(TranslateModel.Create(TranslateId.Of(Guid.NewGuid()), sessionId1, ValueObjects.TranslationConfiguration.Of(Translate.Enums.TranslationStatus.User.ToString()), TranslatedText.Of("Hello, what is AI?"), TokenUsed.Of(10)));
        Translates[0].AddTranslate(TranslateModel.Create(TranslateId.Of(Guid.NewGuid()), sessionId1, ValueObjects.TranslationConfiguration.Of(Translate.Enums.TranslationStatus.System.ToString()), TranslatedText.Of("AI stands for Artificial Intelligence..."), TokenUsed.Of(50)));

        var sessionId2 = SessionId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b"));
        Translates[1].AddTranslate(TranslateModel.Create(TranslateId.Of(Guid.NewGuid()), sessionId2, ValueObjects.TranslationConfiguration.Of(Translate.Enums.TranslationStatus.User.ToString()), TranslatedText.Of("How to make a pizza?"), TokenUsed.Of(12)));
        Translates[1].AddTranslate(TranslateModel.Create(TranslateId.Of(Guid.NewGuid()), sessionId2, ValueObjects.TranslationConfiguration.Of(Translate.Enums.TranslationStatus.System.ToString()), TranslatedText.Of("To make a pizza, you need dough, sauce..."), TokenUsed.Of(100)));
    }
}