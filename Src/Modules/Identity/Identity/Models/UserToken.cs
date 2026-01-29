namespace Identity.Identity.Models;

using System;
using AI.Common.Core;
using Microsoft.AspNetCore.Identity;

public class UserToken : IdentityUserToken<Guid>, IVersion
{
    public long Version { get; set; }
}