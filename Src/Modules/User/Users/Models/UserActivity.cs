using AI.Common.Core;
using User.ValueObjects;

namespace User.Models;

public record UserActivity : Entity<UserActivityId>
{
        ActivityId
UserId
Module
Action
ResourceId
Timestamp
Metadata
    }
