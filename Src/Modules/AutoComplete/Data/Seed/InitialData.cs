namespace AutoComplete.Data.Seed;

using AI.Common.Core;
using global::AutoComplete.Models;
using global::AutoComplete.ValueObjects;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<AutocompleteModel> AutoCompletes { get; } = new()
    {
        AutocompleteModel.Create(SessionId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a")), UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), "Getting started with AI", "gpt-4"),
        AutocompleteModel.Create(SessionId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b")), UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), "Cooking Recipes", "gpt-3.5-turbo")
    };

    static InitialData()
    {
        var sessionId1 = SessionId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a"));
        AutoCompletes[0].AddAutoComplete(AutoCompleteModel.Create(AutoCompleteId.Of(Guid.NewGuid()), sessionId1, AutocompleteConfiguration.Of(AutoComplete.Enums.AutocompleteStatus.User.ToString()), AutocompleteSuggestion.Of("Hello, what is AI?"), ValueObjects.AutoCompletePrompt.Of(10)));
        AutoCompletes[0].AddAutoComplete(AutoCompleteModel.Create(AutoCompleteId.Of(Guid.NewGuid()), sessionId1, AutocompleteConfiguration.Of(AutoComplete.Enums.AutocompleteStatus.System.ToString()), AutocompleteSuggestion.Of("AI stands for Artificial Intelligence..."), ValueObjects.AutoCompletePrompt.Of(50)));

        var sessionId2 = SessionId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b"));
        AutoCompletes[1].AddAutoComplete(AutoCompleteModel.Create(AutoCompleteId.Of(Guid.NewGuid()), sessionId2, AutocompleteConfiguration.Of(AutoComplete.Enums.AutocompleteStatus.User.ToString()), AutocompleteSuggestion.Of("How to make a pizza?"), ValueObjects.AutoCompletePrompt.Of(12)));
        AutoCompletes[1].AddAutoComplete(AutoCompleteModel.Create(AutoCompleteId.Of(Guid.NewGuid()), sessionId2, AutocompleteConfiguration.Of(AutoComplete.Enums.AutocompleteStatus.System.ToString()), AutocompleteSuggestion.Of("To make a pizza, you need dough, sauce..."), ValueObjects.AutoCompletePrompt.Of(100)));
    }
}