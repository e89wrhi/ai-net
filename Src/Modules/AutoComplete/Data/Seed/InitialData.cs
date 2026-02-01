namespace AutoComplete.Data.Seed;

using AI.Common.Core;
using global::AutoComplete.Models;
using global::AutoComplete.ValueObjects;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<AutoCompleteSession> AutoCompletes { get; } = new()
    {
        AutoCompleteSession.Create(
            AutoCompleteId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            ModelId.Of("gpt-4"), 
            AutoCompleteConfiguration.Of("default")
        ),
        AutoCompleteSession.Create(
            AutoCompleteId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            ModelId.Of("gpt-3.5-turbo"), 
            AutoCompleteConfiguration.Of("default")
        )
    };

    static InitialData()
    {
        // AddRequest(AutoCompleteRequest request)
        // AutoCompleteRequest definition is needed to construct it.
        // Assuming AutoCompleteRequest.Create(...) exists or similar.
        // Let's create dummy requests.
        // But first I need to check AutoCompleteRequest signature.
        // Since I can't check it right now, I'll comment out the details or try to guess based on context.
        // Alternatively, I will update this file again after checking AutoCompleteRequest.
    }
}