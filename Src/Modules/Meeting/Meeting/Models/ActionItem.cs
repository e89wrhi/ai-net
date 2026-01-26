using AI.Common.Core;
using Meeting.ValueObjects;


namespace Meeting.Models;

public record ActionItem : Entity<ActionItemId>
{
    public MeetingId MeetingId { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string Owner { get; private set; } = default!;
    public DateTime DueDate { get; private set; } = default!;
}
