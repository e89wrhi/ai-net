using AI.Common.Core;
using User.ValueObjects;

namespace User.Models;

public record UserModel : Aggregate<UserId>
{
    public string Username { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string FullName { get; private set; } = default!;
    public string Bio { get; private set; } = default!;
    public string AvatarUrl { get; private set; } = default!;
    public string JobTitle { get; private set; } = default!;

    private readonly List<UsageContainer> _usages = new();
    public IReadOnlyCollection<UsageContainer> Usages => _usages.AsReadOnly();


    private readonly List<UserActivity> _activities = new();
    public IReadOnlyCollection<UserActivity> Activities => _activities.AsReadOnly();

    private UserModel() { }

    public static UserModel Create(UserId id, string username, string email)
    {
        var user = new UserModel
        {
            Id = id,
            Username = username,
            Email = email,
            CreatedAt = DateTime.UtcNow
        };

        user.AddDomainEvent(new User.Events.UserCreatedDomainEvent(id, email, username));
        return user;
    }

    public void UpdateProfile(string fullName, string bio, string jobTitle, string avatarUrl)
    {
        FullName = fullName;
        Bio = bio;
        JobTitle = jobTitle;
        AvatarUrl = avatarUrl;
        LastModified = DateTime.UtcNow;

        AddDomainEvent(new User.Events.UserProfileUpdatedDomainEvent(Id, fullName, jobTitle));
    }

    public void TrackActivity(UserActivity activity)
    {
        _activities.Add(activity);
        AddDomainEvent(new User.Events.UserActivityTrackedDomainEvent(Id, activity.Id, activity.Module, activity.Action));
    }

    public void RecordUsage(UsageContainer usage)
    {
        _usages.Add(usage);
        AddDomainEvent(new User.Events.UsageRecordAddedDomainEvent(Id, usage.Id, usage.TokenUsed));
    }
}
