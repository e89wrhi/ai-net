using AI.Common.Core;
using Meeting.Enums;
using Meeting.ValueObjects;


namespace Meeting.Models;

public record MeetingModel : Aggregate<MeetingId>
{
    public MeetingStatus MeetingStatus { get; private set; } = default!;
    public Title Title { get; private set; } = default!;
    public Summary Summary { get; private set; } = default!;
    public AudioSource AudioSource { get; private set; } = default!;
    public string OrganizerId { get; private set; } = default!;
    public DateTime? StartTime { get; private set; } = default!;
    public DateTime? EndTime { get; private set; } = default!;
    public string RecordingUrl { get; private set; } = default!;

    private readonly List<ActionItem> _actions = new();
    public IReadOnlyCollection<ActionItem> Actions => _actions.AsReadOnly();


    private readonly List<ParticipantModel> _participants = new();
    public IReadOnlyCollection<ParticipantModel> Participants => _participants.AsReadOnly();

    public string Keypoints { get; private set; } = default!;

    // transcript
    public string Text { get; private set; } = default!;
    public TranscriptLanguage Language { get; private set; } = default!;
    public double ConfidenceScore { get; private set; } = default!;

}
