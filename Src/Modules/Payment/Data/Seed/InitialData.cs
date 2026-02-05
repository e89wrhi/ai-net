namespace Payment.Data.Seed;

using AI.Common.Core;
using global::Payment.Models;
using global::Payment.ValueObjects;
using global::Payment.Enums;
using System;
using System.Collections.Generic;

public static class InitialData
{
    public static List<SubscriptionModel> Subscriptions { get; } = new()
    {
        SubscriptionModel.Create(
            SubscriptionId.Of(Guid.Parse("da8fad5b-d9cb-469f-a165-708677289511")), 
            UserId.Of(Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e")), 
            "Pro", 
            100, 
            100000)
    };

    static InitialData()
    {
    }
}