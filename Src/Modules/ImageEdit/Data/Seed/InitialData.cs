namespace ImageEdit.Data.Seed;

using AI.Common.Core;
using global::ImageEdit.Models;
using global::ImageEdit.ValueObjects;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<ImageEditModel> ImageEdits { get; } = new()
    {
        ImageEditModel.Create(SessionId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a")), UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), "Getting started with AI", "gpt-4"),
        ImageEditModel.Create(SessionId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b")), UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), "Cooking Recipes", "gpt-3.5-turbo")
    };

    static InitialData()
    {
        var sessionId1 = SessionId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a"));
        ImageEdits[0].AddImageEdit(ImageEditModel.Create(ImageEditId.Of(Guid.NewGuid()), sessionId1, ValueObjects.ImageEditPrompt.Of(ImageEdit.Enums.ImageEditStatus.User.ToString()), ValueObjects.EditedImage.Of("Hello, what is AI?"), TokenUsed.Of(10)));
        ImageEdits[0].AddImageEdit(ImageEditModel.Create(ImageEditId.Of(Guid.NewGuid()), sessionId1, ValueObjects.ImageEditPrompt.Of(ImageEdit.Enums.ImageEditStatus.System.ToString()), ValueObjects.EditedImage.Of("AI stands for Artificial Intelligence..."), TokenUsed.Of(50)));

        var sessionId2 = SessionId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b"));
        ImageEdits[1].AddImageEdit(ImageEditModel.Create(ImageEditId.Of(Guid.NewGuid()), sessionId2, ValueObjects.ImageEditPrompt.Of(ImageEdit.Enums.ImageEditStatus.User.ToString()), ValueObjects.EditedImage.Of("How to make a pizza?"), TokenUsed.Of(12)));
        ImageEdits[1].AddImageEdit(ImageEditModel.Create(ImageEditId.Of(Guid.NewGuid()), sessionId2, ValueObjects.ImageEditPrompt.Of(ImageEdit.Enums.ImageEditStatus.System.ToString()), ValueObjects.EditedImage.Of("To make a pizza, you need dough, sauce..."), TokenUsed.Of(100)));
    }
}