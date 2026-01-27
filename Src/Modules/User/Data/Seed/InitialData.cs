namespace User.Data.Seed;

using AI.Common.Core;
using global::User.Models;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;

public static class InitialData
{
    public static List<UserModel> Users { get; }


    static InitialData()
    {
    }
}