using AI.Common.Core;
using User.Enums;
using User.ValueObjects;

namespace User.Models;

public record UserActivitySession : Aggregate<UserActivityId>
{
    public UserId UserId { get; private set; } = default!;
    public UserActivityStatus Status { get; private set; }
    public DateTime LastActivityAt { get; private set; }
    public int TotalActions { get; private set; } = 0;

    private readonly List<UserAction> _actions = new();
    public IReadOnlyCollection<UserAction> Actions => _actions.AsReadOnly();

    private UserActivitySession() { }

    public static UserActivitySession Create(UserActivityId id, UserId userId)
    {
        var session = new UserActivitySession
        {
            Id = id,
            UserId = userId,
            Status = UserActivityStatus.Active,
            CreatedAt = DateTime.UtcNow,
            LastActivityAt = DateTime.UtcNow
        };

        session.AddDomainEvent(
            new Events.UserActivitySessionStartedDomainEvent(id, userId));

        return session;
    }

    public void RecordAction(UserAction action)
    {
        _actions.Add(action);
        LastActivityAt = DateTime.UtcNow;
        TotalActions += 1;

        AddDomainEvent(
            new Events.UserActionRecordedDomainEvent(Id, action.Id, action.ActionType));
    }

    public void Complete()
    {
        Status = UserActivityStatus.Completed;
        AddDomainEvent(new Events.UserActivitySessionCompletedDomainEvent(Id));
    }

    public void Suspend()
    {
        Status = UserActivityStatus.Suspended;
        AddDomainEvent(new Events.UserActivitySessionSuspendedDomainEvent(Id));
    }
}
