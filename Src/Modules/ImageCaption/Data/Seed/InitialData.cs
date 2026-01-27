namespace ImageCaption.Data.Seed;

using AI.Common.Core;
using global::ImageCaption.Models;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;

public static class InitialData
{
    public static List<ImageModel> Images { get; }


    static InitialData()
    {
    }
}