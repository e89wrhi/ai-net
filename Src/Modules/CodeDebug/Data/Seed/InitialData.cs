namespace CodeDebug.Data.Seed;

using AI.Common.Core;
using global::CodeDebug.Models;
using global::CodeDebug.ValueObjects;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<CodeDebugSession> CodeDebugs { get; } = new()
    {
        CodeDebugSession.Create(SessionId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a")), UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), "Getting started with AI", "gpt-4"),
        CodeDebugSession.Create(SessionId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b")), UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), "Cooking Recipes", "gpt-3.5-turbo")
    };

    static InitialData()
    {
        var sessionId1 = SessionId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a"));
        CodeDebugs[0].AddCodeDebug(CodeDebugModel.Create(CodeDebugId.Of(Guid.NewGuid()), sessionId1, CodeDebugSender.Of(CodeDebug.Enums.ProgrammingLanguage.User.ToString()), CodeDebugContent.Of("Hello, what is AI?"), TokenUsed.Of(10)));
        CodeDebugs[0].AddCodeDebug(CodeDebugModel.Create(CodeDebugId.Of(Guid.NewGuid()), sessionId1, CodeDebugSender.Of(CodeDebug.Enums.ProgrammingLanguage.System.ToString()), CodeDebugContent.Of("AI stands for Artificial Intelligence..."), TokenUsed.Of(50)));

        var sessionId2 = SessionId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b"));
        CodeDebugs[1].AddCodeDebug(CodeDebugModel.Create(CodeDebugId.Of(Guid.NewGuid()), sessionId2, CodeDebugSender.Of(CodeDebug.Enums.ProgrammingLanguage.User.ToString()), CodeDebugContent.Of("How to make a pizza?"), TokenUsed.Of(12)));
        CodeDebugs[1].AddCodeDebug(CodeDebugModel.Create(CodeDebugId.Of(Guid.NewGuid()), sessionId2, CodeDebugSender.Of(CodeDebug.Enums.ProgrammingLanguage.System.ToString()), CodeDebugContent.Of("To make a pizza, you need dough, sauce..."), TokenUsed.Of(100)));
    }
}