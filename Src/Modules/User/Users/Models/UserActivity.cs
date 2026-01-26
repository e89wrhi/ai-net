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
    public string IpAddress { get; private set; } = default!;
    public string UserAgent { get; private set; } = default!;

    private UserActivity() { }

    public static UserActivity Create(UserActivityId id, UserId userId, TrackedModule module, string action, Guid resourceId, string ip, string userAgent)
    {
        return new UserActivity
        {
            Id = id,
            UserId = userId,
            Module = module,
            Action = action,
            ResourceId = resourceId,
            IpAddress = ip,
            UserAgent = userAgent,
            TimeStamp = DateTimeOffset.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
    }
}