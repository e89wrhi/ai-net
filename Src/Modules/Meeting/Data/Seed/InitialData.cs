namespace Meeting.Data.Seed;

using AI.Common.Core;
using global::Meeting.Models;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;

public static class InitialData
{
    public static List<MeetingModel> Meetings { get; }


    static InitialData()
    {
    }
}