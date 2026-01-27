namespace ChatBot.Data.Seed;

using AI.Common.Core;
using global::ChatBot.Models;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;

public static class InitialData
{
    public static List<ChatModel> Chats { get; }


    static InitialData()
    {
    }
}