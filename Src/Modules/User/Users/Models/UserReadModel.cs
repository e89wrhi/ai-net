namespace User.Models;

public class UserReadModel
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public List<UserActivityReadModel> Activities { get; set; } = new();
    public List<UsageContainerReadModel> Usages { get; set; } = new();
}

public class UserActivityReadModel
{
    public Guid Id { get; set; }
    public string Module { get; set; } = default!;
    public string Action { get; set; } = default!;
    public Guid ResourceId { get; set; }
    public DateTimeOffset TimeStamp { get; set; }
}

public class UsageContainerReadModel
{
    public Guid Id { get; set; }
    public string Period { get; set; } = default!;
    public string TokenUsed { get; set; } = default!;
    public int RequestsCount { get; set; }
}

