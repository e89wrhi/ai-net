namespace Resume.Dtos;

public class ResumeSuggestionDto
{
    public Guid Id { get; set; }
    public string Section { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Impact { get; set; } = default!;
}

