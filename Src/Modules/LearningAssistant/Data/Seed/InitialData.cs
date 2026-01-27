namespace LearningAssistant.Data.Seed;

using AI.Common.Core;
using global::LearningAssistant.Models;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;

public static class InitialData
{
    public static List<ProfileModel> Profiles { get; }


    static InitialData()
    {
    }
}