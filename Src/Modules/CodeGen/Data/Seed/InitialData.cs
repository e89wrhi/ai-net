namespace CodeGen.Data.Seed;

using AI.Common.Core;
using global::CodeGen.Models;
using global::CodeGen.ValueObjects;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<CodeGenModel> CodeGens { get; } = new()
    {
        CodeGenModel.Create(SessionId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a")), UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), "Getting started with AI", "gpt-4"),
        CodeGenModel.Create(SessionId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b")), UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), "Cooking Recipes", "gpt-3.5-turbo")
    };

    static InitialData()
    {
        var sessionId1 = SessionId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a"));
        CodeGens[0].AddCodeGen(CodeGenModel.Create(CodeGenId.Of(Guid.NewGuid()), sessionId1, ValueObjects.CodeGenerationPrompt.Of(CodeGen.Enums.CodeGenerationStatus.User.ToString()), IssueCount.Of("Hello, what is AI?"), ValueObjects.CodeGenerationConfiguration.Of(10)));
        CodeGens[0].AddCodeGen(CodeGenModel.Create(CodeGenId.Of(Guid.NewGuid()), sessionId1, ValueObjects.CodeGenerationPrompt.Of(CodeGen.Enums.CodeGenerationStatus.System.ToString()), IssueCount.Of("AI stands for Artificial Intelligence..."), ValueObjects.CodeGenerationConfiguration.Of(50)));

        var sessionId2 = SessionId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b"));
        CodeGens[1].AddCodeGen(CodeGenModel.Create(CodeGenId.Of(Guid.NewGuid()), sessionId2, ValueObjects.CodeGenerationPrompt.Of(CodeGen.Enums.CodeGenerationStatus.User.ToString()), IssueCount.Of("How to make a pizza?"), ValueObjects.CodeGenerationConfiguration.Of(12)));
        CodeGens[1].AddCodeGen(CodeGenModel.Create(CodeGenId.Of(Guid.NewGuid()), sessionId2, ValueObjects.CodeGenerationPrompt.Of(CodeGen.Enums.CodeGenerationStatus.System.ToString()), IssueCount.Of("To make a pizza, you need dough, sauce..."), ValueObjects.CodeGenerationConfiguration.Of(100)));
    }
}