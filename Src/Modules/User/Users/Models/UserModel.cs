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
}
