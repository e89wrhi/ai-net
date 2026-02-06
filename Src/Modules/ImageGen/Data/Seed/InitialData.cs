namespace ImageGen.Data.Seed;

using AI.Common.Core;
using AiOrchestration.ValueObjects;
using global::ImageGen.Models;
using global::ImageGen.ValueObjects;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<ImageGenerationSession> ImageGens { get; } = new()
    {
        ImageGenerationSession.Create(
            ImageGenId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            ModelId.Of("dall-e-3"),
            new ImageGenerationConfiguration(global::ImageGen.Enums.ImageSize.Large, global::ImageGen.Enums.ImageStyle.Realistic, global::ImageGen.Enums.ImageQuality.High, LanguageCode.Of("en"))),
        ImageGenerationSession.Create(
            ImageGenId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            ModelId.Of("dall-e-3"),
            new ImageGenerationConfiguration(global::ImageGen.Enums.ImageSize.Medium, global::ImageGen.Enums.ImageStyle.DigitalArt, global::ImageGen.Enums.ImageQuality.Medium, LanguageCode.Of("en")))
    };

    static InitialData()
    {
        ImageGens[0].AddResult(ImageGenerationResult.Create(
            ImageGenResultId.Of(Guid.NewGuid()), 
            ImageGenerationPrompt.Of("A futuristic cityscape at sunset"),
            GeneratedImage.Of("https://example.com/generated1.png"),
            global::ImageGen.Enums.ImageSize.Large,
            global::ImageGen.Enums.ImageStyle.Realistic,
            TokenCount.Of(250),
            AiOrchestration.ValueObjects.CostEstimate.Of(0.04m)));
            
        ImageGens[1].AddResult(ImageGenerationResult.Create(
            ImageGenResultId.Of(Guid.NewGuid()), 
            ImageGenerationPrompt.Of("A serene mountain landscape"),
            GeneratedImage.Of("https://example.com/generated2.png"),
            global::ImageGen.Enums.ImageSize.Medium,
            global::ImageGen.Enums.ImageStyle.DigitalArt,
            TokenCount.Of(200),
            AiOrchestration.ValueObjects.CostEstimate.Of(0.03m)));
    }
}