namespace Meeting.Models;

public class MeetingAnalysisSessionReadModel
{
    public Guid Id { get; set; }
    public string OrganizerId { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Summary { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string Transcript { get; set; } = default!;
    public List<string> ActionItems { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

