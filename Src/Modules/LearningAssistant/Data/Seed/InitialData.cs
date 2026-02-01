namespace LearningAssistant.Data.Seed;

using LearningAssistant.Models;
using LearningAssistant.ValueObjects;
using LearningAssistant.Enums;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<ProfileModel> Profiles { get; } = new()
    {
        ProfileModel.Create(ActivityId.Of(Guid.Parse("ba8fad5b-d9cb-469f-a165-70867728950f")), "0f8fad5b-d9cb-469f-a165-70867728950e", "Visual", "Beginner")
    };

    static InitialData()
    {
        var profileId = ActivityId.Of(Guid.Parse("ba8fad5b-d9cb-469f-a165-70867728950f"));
        var lessonId = LearningId.Of(Guid.NewGuid());
        var lesson = LearningActivityItem.Create(lessonId, profileId, "Introduction to Machine Learning", "Machine learning is a subset of AI...", DifficultyLevel.Beginner);
        
        var quiz = QuizModel.Create(QuizId.Of(Guid.NewGuid()), lessonId, "What is Machine Learning?");
        lesson.AddQuiz(quiz);
        
        Profiles[0].AddLesson(lesson);
    }
}