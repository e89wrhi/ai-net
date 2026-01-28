namespace User.Data.Seed;

using AI.Common.Core;
using global::User.Models;
using global::User.ValueObjects;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<UserModel> Users { get; } = new()
    {
        UserModel.Create(UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), "john_doe", "john@example.com"),
        UserModel.Create(UserId.Of(Guid.Parse("1d8fad5b-d9cb-469f-a165-70867728950f")), "jane_smith", "jane@example.com")
    };

    static InitialData()
    {
        Users[0].UpdateProfile("John Doe", "AI Enthusiast", "Software Engineer", "https://api.dicebear.com/7.x/avataaars/svg?seed=John");
        Users[1].UpdateProfile("Jane Smith", "Data Scientist", "Lead AI Researcher", "https://api.dicebear.com/7.x/avataaars/svg?seed=Jane");
    }
}