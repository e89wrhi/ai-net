namespace Resume.Dtos;

public class ResumeAnalysisDto
{
    public Guid Id { get; set; }
    public string CandidateName { get; set; } = default!;
    public string Summary { get; set; } = default!;
    public string Status { get; set; } = default!;
    public DateTime? AnalyzedAt { get; set; }
}

