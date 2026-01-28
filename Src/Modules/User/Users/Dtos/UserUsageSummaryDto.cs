namespace User.Dtos;

public class UserUsageSummaryDto
{
    public Guid Id { get; set; }
    public string Period { get; set; } = default!;
    public string TokenUsed { get; set; } = default!;
    public int RequestsCount { get; set; }
}

