using AI.Common.Core;
using User.ValueObjects;

namespace User.Models;

public record UserAction : Entity<UserActionId>
{
    public string ActionType { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public DateTime PerformedAt { get; private set; }

    private UserAction() { }

    public static UserAction Create(string actionType, string description)
    {
        return new UserAction
        {
            Id = UserActionId.Of(Guid.NewGuid()),
            ActionType = actionType,
            Description = description,
            PerformedAt = DateTime.UtcNow
        };
    }
}
