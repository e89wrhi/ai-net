namespace Payment.Data.Seed;

using AI.Common.Core;
using global::Payment.Models;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;

public static class InitialData
{
    public static List<SubscriptionModel> Subscriptions { get; }


    static InitialData()
    {
    }
}