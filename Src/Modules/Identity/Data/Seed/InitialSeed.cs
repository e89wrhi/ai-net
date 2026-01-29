namespace Identity.Data.Seed;

using System;
using System.Collections.Generic;
using Identity.Models;
using MassTransit;

public static class InitialData
{
    public static List<User> Users { get; }

    static InitialData()
    {
        Users = new List<User>
        {
            new User
            {
                Id = NewId.NextGuid(),
                FirstName = "Will",
                LastName = "IAM",
                UserName = "willi",
                PassPortNumber = "12345678",
                Email = "will@test.com",
                SecurityStamp = Guid.NewGuid().ToString()
            },
            new User
            {
                Id = NewId.NextGuid(),
                FirstName = "Dani",
                LastName = "IO",
                UserName = "danio",
                PassPortNumber = "87654321",
                Email = "dani@test.com",
                SecurityStamp = Guid.NewGuid().ToString()
            }
        };
    }
}