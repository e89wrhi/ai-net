namespace User.Dtos;

public class UserActivityDto
{
    public Guid Id { get; set; }
    public string Module { get; set; } = default!;
    public string Action { get; set; } = default!;
    public Guid ResourceId { get; set; }
    public DateTimeOffset TimeStamp { get; set; }
}

