namespace Payment.Dtos;

public class SubscriptionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Plan { get; set; } = default!;
    public string Status { get; set; } = default!;
    public DateTime? StartedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

