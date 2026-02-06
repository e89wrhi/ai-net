namespace SpeechToText.Data.Seed;

using AI.Common.Core;
using AiOrchestration.ValueObjects;
using global::SpeechToText.Models;
using global::SpeechToText.ValueObjects;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<SpeechToTextSession> SpeechToTexts { get; } = new()
    {
        SpeechToTextSession.Create(
            SpeechToTextId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            ModelId.Of("whisper-1"),
            new SpeechToTextConfiguration(LanguageCode.Of("en"), true, global::SpeechToText.Enums.SpeechToTextDetailLevel.Standard)),
        SpeechToTextSession.Create(
            SpeechToTextId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            ModelId.Of("whisper-1"),
            new SpeechToTextConfiguration(LanguageCode.Of("es"), true, global::SpeechToText.Enums.SpeechToTextDetailLevel.Detailed))
    };

    static InitialData()
    {
        SpeechToTexts[0].AddResult(SpeechToTextResult.Create(
            SpeechToTextResultId.Of(Guid.NewGuid()), 
            AudioSource.Of("https://example.com/audio1.mp3"),
            Transcript.Of("Hello, how are you today?"),
            TokenCount.Of(60),
            CostEstimate.Of(0.006m)));
            
        SpeechToTexts[1].AddResult(SpeechToTextResult.Create(
            SpeechToTextResultId.Of(Guid.NewGuid()), 
            AudioSource.Of("https://example.com/audio2.mp3"),
            Transcript.Of("Hola, ¿cómo estás hoy?"),
            TokenCount.Of(55),
            CostEstimate.Of(0.0055m)));
    }
}