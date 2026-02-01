namespace ImageCaption.Data.Seed;

using AI.Common.Core;
using global::ImageCaption.Models;
using global::ImageCaption.ValueObjects;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<ImageModel> Images { get; } = new()
    {
        ImageModel.Create(ImageCaptionResultId.Of(Guid.Parse("9a8fad5b-d9cb-469f-a165-70867728950c")), "0f8fad5b-d9cb-469f-a165-70867728950e", ValueObjects.CaptionConfidence.Of("uploads/img1.jpg", "filename.jpg"), 1920, 1080, 500000, "jpg"),
        ImageModel.Create(ImageCaptionResultId.Of(Guid.Parse("aa8fad5b-d9cb-469f-a165-70867728950d")), "0f8fad5b-d9cb-469f-a165-70867728950e", ValueObjects.CaptionConfidence.Of("uploads/img2.png", "filename.png"), 800, 600, 200000, "png")
    };

    static InitialData()
    {
        var imageId1 = ImageCaptionResultId.Of(Guid.Parse("9a8fad5b-d9cb-469f-a165-70867728950c"));
        Images[0].AddCaption(CaptionModel.Create(ImageCaptionId.Of(Guid.NewGuid()), imageId1, "A beautiful sunset over the ocean", 0.98, "en"));

        var imageId2 = ImageCaptionResultId.Of(Guid.Parse("aa8fad5b-d9cb-469f-a165-70867728950d"));
        Images[1].AddCaption(CaptionModel.Create(ImageCaptionId.Of(Guid.NewGuid()), imageId2, "A group of people working in an office", 0.95, "en"));
    }
}