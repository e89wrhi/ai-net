namespace ImageCaption.Data.Seed;

using AI.Common.Core;
using AiOrchestration.ValueObjects;
using global::ImageCaption.Models;
using global::ImageCaption.ValueObjects;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<ImageCaptionSession> Images { get; } = new()
    {
        ImageCaptionSession.Create(
            ImageCaptionId.Of(Guid.Parse("9a8fad5b-d9cb-469f-a165-70867728950c")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            ModelId.Of("gpt-4-vision"),
            new ImageCaptionConfiguration(global::ImageCaption.Enums.CaptionDetailLevel.Standard, LanguageCode.Of("en"))),
        ImageCaptionSession.Create(
            ImageCaptionId.Of(Guid.Parse("aa8fad5b-d9cb-469f-a165-70867728950d")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            ModelId.Of("gpt-4-vision"),
            new ImageCaptionConfiguration(global::ImageCaption.Enums.CaptionDetailLevel.Detailed, LanguageCode.Of("en")))
    };

    static InitialData()
    {
        Images[0].AddResult(ImageCaptionResult.Create(
            ImageCaptionResultId.Of(Guid.NewGuid()), 
            ImageSource.Of("https://example.com/sunset.jpg"),
            CaptionText.Of("A beautiful sunset over the ocean"),
            CaptionConfidence.Of(0.98f),
            TokenCount.Of(100),
            CostEstimate.Of(0.002m)));

        Images[1].AddResult(ImageCaptionResult.Create(
            ImageCaptionResultId.Of(Guid.NewGuid()), 
            ImageSource.Of("https://example.com/office.jpg"),
            CaptionText.Of("A group of people working in an office"),
            CaptionConfidence.Of(0.95f),
            TokenCount.Of(120),
            CostEstimate.Of(0.0024m)));
    }
}