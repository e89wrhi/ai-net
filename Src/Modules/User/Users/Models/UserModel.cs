using AI.Common.Core;
using User.ValueObjects;

namespace User.Models;

public record UserModel : Aggregate<UserId>
{

    private readonly List<UsageContainer> _usages = new();
    public IReadOnlyCollection<UsageContainer> Usages => _usages.AsReadOnly();


    private readonly List<UserActivity> _activities = new();
    public IReadOnlyCollection<UserActivity> Activities => _activities.AsReadOnly();
}
