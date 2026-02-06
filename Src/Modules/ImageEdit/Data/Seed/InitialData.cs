namespace ImageEdit.Data.Seed;

using AI.Common.Core;
using AiOrchestration.ValueObjects;
using global::ImageEdit.Models;
using global::ImageEdit.ValueObjects;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<ImageEditSession> ImageEdits { get; } = new()
    {
        ImageEditSession.Create(
            ImageEditId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            ModelId.Of("dall-e-2"),
            new ImageEditConfiguration(global::ImageEdit.Enums.ImageEditQuality.High, global::ImageEdit.Enums.ImageFormat.Png)),
        ImageEditSession.Create(
            ImageEditId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            ModelId.Of("dall-e-2"),
            new ImageEditConfiguration(global::ImageEdit.Enums.ImageEditQuality.Medium, global::ImageEdit.Enums.ImageFormat.Jpeg))
    };

    static InitialData()
    {
        ImageEdits[0].AddResult(ImageEditResult.Create(
            ImageEditResultId.Of(Guid.NewGuid()), 
            ImageSource.Of("https://example.com/original1.jpg"),
            EditedImage.Of("https://example.com/edited1.png"),
            ImageEditPrompt.Of("Add a sunset background"),
            global::ImageEdit.Enums.EditOperation.ObjectInsertion,
            TokenCount.Of(200),
            AiOrchestration.ValueObjects.CostEstimate.Of(0.02m)));
            
        ImageEdits[1].AddResult(ImageEditResult.Create(
            ImageEditResultId.Of(Guid.NewGuid()), 
            ImageSource.Of("https://example.com/original2.jpg"),
            EditedImage.Of("https://example.com/edited2.jpg"),
            ImageEditPrompt.Of("Remove background"),
            global::ImageEdit.Enums.EditOperation.BackgroundRemoval,
            TokenCount.Of(180),
            AiOrchestration.ValueObjects.CostEstimate.Of(0.018m)));
    }
}