namespace Resume.Models;

public class ResumeReadModel
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = default!;
    public string CandidateName { get; set; } = default!;
    public string FilePath { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string Summary { get; set; } = default!;
    public string ParsedText { get; set; } = default!;
    public List<string> Skills { get; set; } = new();
    public List<string> Suggestions { get; set; } = new();
    public DateTime? AnalyzedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

