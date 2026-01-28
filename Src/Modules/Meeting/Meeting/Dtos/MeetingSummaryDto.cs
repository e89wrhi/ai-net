namespace Meeting.Dtos;

public class MeetingSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Summary { get; set; } = default!;
    public string Status { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}

