namespace Resume.Data.Seed;

using AI.Common.Core;
using AiOrchestration.ValueObjects;
using global::Resume.Models;
using global::Resume.ValueObjects;
using global::Resume.Enums;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<ResumeAnalysisSession> Resumes { get; } = new()
    {
        ResumeAnalysisSession.Create(
            ResumeId.Of(Guid.Parse("ea8fad5b-d9cb-469f-a165-708677289512")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            ModelId.Of("gpt-4"),
            new ResumeAnalysisConfiguration(true, true, true))
    };

    static InitialData()
    {
        Resumes[0].AddResult(ResumeAnalysisResult.Create(
            ResultId.Of(Guid.NewGuid()), 
            ResumeFile.Of("https://example.com/john_doe_resume.pdf", "john_doe_resume.pdf"),
            ResumeSummary.Of("Experienced software engineer with 5+ years in AI and machine learning. Strong background in C# and Python development."),
            CandidateScore.Of(0.92f),
            TokenCount.Of(300),
            AiOrchestration.ValueObjects.CostEstimate.Of(0.006m)));
    }
}