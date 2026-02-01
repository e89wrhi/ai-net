namespace ImageGen.Data.Seed;

using AI.Common.Core;
using global::ImageGen.Models;
using global::ImageGen.ValueObjects;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<ImageGenModel> ImageGens { get; } = new()
    {
        ImageGenModel.Create(SessionId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a")), UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), "Getting started with AI", "gpt-4"),
        ImageGenModel.Create(SessionId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b")), UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), "Cooking Recipes", "gpt-3.5-turbo")
    };

    static InitialData()
    {
        var sessionId1 = SessionId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a"));
        ImageGens[0].AddImageGen(ImageGenModel.Create(ImageGenId.Of(Guid.NewGuid()), sessionId1, ValueObjects.ImageGenerationPrompt.Of(ImageGen.Enums.ImageGenerationStatus.User.ToString()), ValueObjects.GeneratedImage.Of("Hello, what is AI?"), TokenUsed.Of(10)));
        ImageGens[0].AddImageGen(ImageGenModel.Create(ImageGenId.Of(Guid.NewGuid()), sessionId1, ValueObjects.ImageGenerationPrompt.Of(ImageGen.Enums.ImageGenerationStatus.System.ToString()), ValueObjects.GeneratedImage.Of("AI stands for Artificial Intelligence..."), TokenUsed.Of(50)));

        var sessionId2 = SessionId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b"));
        ImageGens[1].AddImageGen(ImageGenModel.Create(ImageGenId.Of(Guid.NewGuid()), sessionId2, ValueObjects.ImageGenerationPrompt.Of(ImageGen.Enums.ImageGenerationStatus.User.ToString()), ValueObjects.GeneratedImage.Of("How to make a pizza?"), TokenUsed.Of(12)));
        ImageGens[1].AddImageGen(ImageGenModel.Create(ImageGenId.Of(Guid.NewGuid()), sessionId2, ValueObjects.ImageGenerationPrompt.Of(ImageGen.Enums.ImageGenerationStatus.System.ToString()), ValueObjects.GeneratedImage.Of("To make a pizza, you need dough, sauce..."), TokenUsed.Of(100)));
    }
}