namespace Identity.Identity.Models;

using System;
using Microsoft.AspNetCore.Identity;
using AI.Common.Core;

public class Role : IdentityRole<Guid>, IVersion
{
    public long Version { get; set; }
}