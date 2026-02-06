namespace TextToSpeech.Data.Seed;

using AI.Common.Core;
using AiOrchestration.ValueObjects;
using global::TextToSpeech.Models;
using global::TextToSpeech.ValueObjects;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<TextToSpeechSession> TextToSpeechs { get; } = new()
    {
        TextToSpeechSession.Create(
            TextToSpeechId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            ModelId.Of("tts-1"),
            new TextToSpeechConfiguration(global::TextToSpeech.Enums.VoiceType.Female, global::TextToSpeech.Enums.SpeechSpeed.Normal, LanguageCode.Of("en"))),
        TextToSpeechSession.Create(
            TextToSpeechId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            ModelId.Of("tts-1"),
            new TextToSpeechConfiguration(global::TextToSpeech.Enums.VoiceType.Neutral, global::TextToSpeech.Enums.SpeechSpeed.Fast, LanguageCode.Of("es")))
    };

    static InitialData()
    {
        TextToSpeechs[0].AddResult(TextToSpeechResult.Create(
            TextToSpeechResultId.Of(Guid.NewGuid()), 
            "Hello, welcome to our service!",
            SynthesizedSpeech.Of("https://example.com/speech1.mp3"),
            TokenCount.Of(70),
            CostEstimate.Of(0.015m)));
            
        TextToSpeechs[1].AddResult(TextToSpeechResult.Create(
            TextToSpeechResultId.Of(Guid.NewGuid()), 
            "Hola, bienvenido a nuestro servicio!",
            SynthesizedSpeech.Of("https://example.com/speech2.mp3"),
            TokenCount.Of(75),
            CostEstimate.Of(0.016m)));
    }
}