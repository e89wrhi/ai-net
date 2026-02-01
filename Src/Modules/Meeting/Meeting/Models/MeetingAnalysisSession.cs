using AI.Common.BaseExceptions;
using AI.Common.Core;
using AiOrchestration.ValueObjects;
using Meeting.Enums;
using Meeting.ValueObjects;


namespace Meeting.Models;

public record MeetingAnalysisSession : Aggregate<MeetingId>
{
    public UserId UserId { get; private set; } = default!;
    public ModelId AiModelId { get; private set; } = default!;
    public MeetingAnalysisStatus Status { get; private set; }
    public MeetingAnalysisConfiguration Configuration { get; private set; } = default!;
    public TokenCount TotalTokens { get; private set; } = default!;
    public CostEstimate TotalCost { get; private set; } = default!;
    public DateTime LastAnalyzedAt { get; private set; }

    private readonly List<MeetingTranscript> _transcripts = new();
    public IReadOnlyCollection<MeetingTranscript> Transcripts => _transcripts.AsReadOnly();

    private MeetingAnalysisSession() { }

    public static MeetingAnalysisSession Create(
        MeetingId id,
        UserId userId,
        ModelId aiModelId,
        MeetingAnalysisConfiguration configuration)
    {
        var session = new MeetingAnalysisSession
        {
            Id = id,
            UserId = userId,
            AiModelId = aiModelId,
            Configuration = configuration,
            Status = MeetingAnalysisStatus.Active,
            CreatedAt = DateTime.UtcNow,
            LastAnalyzedAt = DateTime.UtcNow
        };

        session.AddDomainEvent(
            new Events.MeetingAnalysisSessionStartedDomainEvent(id, userId, aiModelId));

        return session;
    }

    public void AddTranscript(MeetingTranscript transcript)
    {
        if (Status != MeetingAnalysisStatus.Active)
            throw new DomainException("Meeting analysis session is not active.");

        _transcripts.Add(transcript);
        LastAnalyzedAt = DateTime.UtcNow;
        var totalcount = TotalTokens.Value;
        TotalTokens = TokenCount.Of(totalcount += transcript.TokenUsed);
        var totalcost = TotalCost.Value;
        TotalCost = CostEstimate.Of(totalcost += transcript.Cost);

        AddDomainEvent(
            new Events.MeetingTranscriptAnalyzedDomainEvent(
                Id, transcript.Id, transcript.Summary.Value));
    }

    public void Complete()
    {
        Status = MeetingAnalysisStatus.Completed;
        AddDomainEvent(new Events.MeetingAnalysisSessionCompletedDomainEvent(Id));
    }

    public void Fail(MeetingAnalysisFailureReason reason)
    {
        Status = MeetingAnalysisStatus.Failed;
        AddDomainEvent(new Events.MeetingAnalysisSessionFailedDomainEvent(Id, reason));
    }
}
