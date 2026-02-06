namespace User.Data.Seed;

using AI.Common.Core;
using global::User.Models;
using global::User.ValueObjects;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<UserActivitySession> Users { get; } = new()
    {
        UserActivitySession.Create(
            UserActivityId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")))
    };

    static InitialData()
    {
        Users[0].RecordAction(UserAction.Create("Login", "User logged in successfully"));
        Users[0].RecordAction(UserAction.Create("ViewProfile", "User viewed their profile"));
    }
}