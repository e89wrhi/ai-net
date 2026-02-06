namespace CodeDebug.Data.Seed;

using AI.Common.Core;
using AiOrchestration.ValueObjects;
using global::CodeDebug.Models;
using global::CodeDebug.ValueObjects;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<CodeDebugSession> CodeDebugs { get; } = new()
    {
        CodeDebugSession.Create(
            CodeDebugId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            ModelId.Of("gpt-4"),
            new global::CodeDebug.ValueObjects.CodeDebugConfiguration(global::CodeDebug.Enums.DebugDepth.Standard, true)),
        CodeDebugSession.Create(
            CodeDebugId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            ModelId.Of("gpt-3.5-turbo"),
            new global::CodeDebug.ValueObjects.CodeDebugConfiguration(global::CodeDebug.Enums.DebugDepth.Standard, true))
    };

    static InitialData()
    {
        CodeDebugs[0].AddReport(CodeDebugReport.Create(
            CodeDebugReportId.Of(Guid.NewGuid()), 
            SourceCode.Of("public void Foo() { }"),
            global::CodeDebug.Enums.ProgrammingLanguage.CSharp,
            DebugSummary.Of("Clean code"),
            IssueCount.Of(0),
            TokenCount.Of(120),
            CostEstimate.Of(0.002m)));
            
        CodeDebugs[1].AddReport(CodeDebugReport.Create(
            CodeDebugReportId.Of(Guid.NewGuid()), 
            SourceCode.Of("print('Hello World')"),
            global::CodeDebug.Enums.ProgrammingLanguage.Python,
            DebugSummary.Of("Good script"),
            IssueCount.Of(0),
            TokenCount.Of(80),
            CostEstimate.Of(0.001m)));
    }
}