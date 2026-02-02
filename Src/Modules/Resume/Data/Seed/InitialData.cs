namespace Resume.Data.Seed;

using AI.Common.Core;
using global::Resume.Models;
using global::Resume.ValueObjects;
using global::Resume.Enums;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<ResumeAnalysisSession> Resumes { get; } = new()
    {
        ResumeModel.Create(ResumeId.Of(Guid.Parse("ea8fad5b-d9cb-469f-a165-708677289512")), "0f8fad5b-d9cb-469f-a165-70867728950e", ValueObjects.ResumeAnalysisConfiguration.Of("John Doe"), ValueObjects.ResumeFile.Of("resumes/john_doe_resume.pdf", "filename.pdf"))
    };

    static InitialData()
    {
        var resumeId = ResumeId.Of(Guid.Parse("ea8fad5b-d9cb-469f-a165-708677289512"));
        Resumes[0].AddSkill(SkillModel.Create(SkillId.Of(Guid.NewGuid()), resumeId, "C#", SkillCategory.Technical, CandidateScore.Of(0.95)));
        Resumes[0].AddSkill(SkillModel.Create(SkillId.Of(Guid.NewGuid()), resumeId, "Python", SkillCategory.Technical, CandidateScore.Of(0.90)));

        Resumes[0].AddSuggestion(SuggestionModel.Create(SuggestionId.Of(Guid.NewGuid()), resumeId, Enums.ResumeAnalysisFailureReason.Formatting, "Add more details about your AI projects", 1));
        
        Resumes[0].CompleteAnalysis("Senior Software Engineer with strong AI background", ParsedText.Of("John Doe... Experience... Education..."));
    }
}