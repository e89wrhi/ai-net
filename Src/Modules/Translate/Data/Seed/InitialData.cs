namespace Translate.Data.Seed;

using AI.Common.Core;
using AiOrchestration.ValueObjects;
using global::Translate.Models;
using global::Translate.ValueObjects;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<TranslationSession> Translates { get; } = new()
    {
        TranslationSession.Create(
            TranslateId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            ModelId.Of("gpt-4"),
            new TranslationConfiguration(LanguageCode.Of("en"), LanguageCode.Of("es"), global::Translate.Enums.TranslationDetailLevel.Standard)),
        TranslationSession.Create(
            TranslateId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            ModelId.Of("gpt-3.5-turbo"),
            new TranslationConfiguration(LanguageCode.Of("en"), LanguageCode.Of("fr"), global::Translate.Enums.TranslationDetailLevel.Detailed))
    };

    static InitialData()
    {
        Translates[0].AddResult(TranslationResult.Create(
            TranslateResultId.Of(Guid.NewGuid()), 
            "Hello, how are you?",
            TranslatedText.Of("Hola, ¿cómo estás?"),
            TokenCount.Of(40),
            CostEstimate.Of(0.0008m)));
            
        Translates[1].AddResult(TranslationResult.Create(
            TranslateResultId.Of(Guid.NewGuid()), 
            "Good morning!",
            TranslatedText.Of("Bonjour!"),
            TokenCount.Of(30),
            CostEstimate.Of(0.0006m)));
    }
}