namespace LearningAssistant.Data.Seed;

using AI.Common.Core;
using global::LearningAssistant.Models;
using global::LearningAssistant.ValueObjects;
using global::LearningAssistant.Enums;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<ProfileModel> Profiles { get; } = new()
    {
        ProfileModel.Create(ProfileId.Of(Guid.Parse("ba8fad5b-d9cb-469f-a165-70867728950f")), "0f8fad5b-d9cb-469f-a165-70867728950e", "Visual", "Beginner")
    };

    static InitialData()
    {
        var profileId = ProfileId.Of(Guid.Parse("ba8fad5b-d9cb-469f-a165-70867728950f"));
        var lessonId = LessonId.Of(Guid.NewGuid());
        var lesson = LessonModel.Create(lessonId, profileId, "Introduction to Machine Learning", "Machine learning is a subset of AI...", DifficultyLevel.Beginner);
        
        var quiz = QuizeModel.Create(QuizeId.Of(Guid.NewGuid()), lessonId, "What is Machine Learning?");
        lesson.AddQuize(quiz);
        
        Profiles[0].AddLesson(lesson);
    }
}