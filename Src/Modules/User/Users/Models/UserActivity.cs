using AI.Common.Core;
using User.Enums;
using User.ValueObjects;

namespace User.Models;

public record UserActivity : Entity<UserActivityId>
{
    public UserId UserId { get; private set; } = default!;
    public TrackedModule Module { get; private set; } = default!;
    public string Action { get; private set; } = default!;
    public Guid ResourceId { get; private set; } = default!;
    public DateTimeOffset TimeStamp { get; private set; } = default!;
    public ActivityMetadata Metadata { get; private set; } = default!;
}