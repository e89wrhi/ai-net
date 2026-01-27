namespace Resume.Data.Seed;

using AI.Common.Core;
using global::Resume.Models;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;

public static class InitialData
{
    public static List<ResumeModel> Resumes { get; }


    static InitialData()
    {
    }
}