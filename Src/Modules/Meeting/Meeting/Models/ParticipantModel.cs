using AI.Common.Core;
using Meeting.ValueObjects;

namespace Meeting.Models;

public record ParticipantModel : Entity<ParticipantId>
{
    public MeetingId MeetingId { get; private set; } = default!;
    public string Name { get; private set; } = default!;
}
