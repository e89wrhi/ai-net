using AI.Common.Core;
using Resume.ValueObjects;

namespace Resume.Models;

public record ExpirenceModel : Entity<ExpirenceId>
{
    public ResumeId ResumeId { get; private set; } = default!;
    public string Company { get; private set; } = default!;

    public string Role { get; private set; } = default!;

    public DateTime? StartedAt { get; private set; } = default!;

    public DateTime? EndedAt { get; private set; } = default!;
}
