namespace CodeGen.Data.Seed;

using AI.Common.Core;
using AiOrchestration.ValueObjects;
using global::CodeGen.Models;
using global::CodeGen.ValueObjects;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<CodeGenerationSession> CodeGens { get; } = new()
    {
        CodeGenerationSession.Create(
            CodeGenId.Of(Guid.Parse("7a8fad5b-d9cb-469f-a165-70867728950a")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            ModelId.Of("gpt-4"),
            new CodeGenerationConfiguration(Temperature.Of(0.7f), TokenCount.Of(2000), global::CodeGen.Enums.CodeStyle.Enterprise, true)),
        CodeGenerationSession.Create(
            CodeGenId.Of(Guid.Parse("8a8fad5b-d9cb-469f-a165-70867728950b")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            ModelId.Of("gpt-3.5-turbo"),
            new CodeGenerationConfiguration(Temperature.Of(0.7f), TokenCount.Of(2000), global::CodeGen.Enums.CodeStyle.Enterprise, true))
    };

    static InitialData()
    {
        CodeGens[0].AddResult(CodeGenerationResult.Create(
            CodeGenResultId.Of(Guid.NewGuid()), 
            CodeGenerationPrompt.Of("Write a C# class for a User"),
            GeneratedCode.Of("public class User { public string Name { get; set; } }"),
            global::CodeGen.ValueObjects.ProgrammingLanguage.Of("CSharp"),
            global::CodeGen.Enums.CodeQualityLevel.Optimized,
            TokenCount.Of(150),
            CostEstimate.Of(0.003m)));
            
        CodeGens[1].AddResult(CodeGenerationResult.Create(
            CodeGenResultId.Of(Guid.NewGuid()), 
            CodeGenerationPrompt.Of("Write a Python function to add two numbers"),
            GeneratedCode.Of("def add(a, b): return a + b"),
            global::CodeGen.ValueObjects.ProgrammingLanguage.Of("Python"),
            global::CodeGen.Enums.CodeQualityLevel.ProductionReady,
            TokenCount.Of(80),
            CostEstimate.Of(0.001m)));
    }
}