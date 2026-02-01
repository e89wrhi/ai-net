using Meeting.Exceptions;

namespace Meeting.ValueObjects;

public record MeetingAnalysisConfiguration
{
    public bool IncludeActionItems { get; }
    public bool IncludeDecisions { get; }
    public LanguageCode Language { get; }

    public MeetingAnalysisConfiguration(bool includeActionItems, bool includeDecisions, LanguageCode language)
    {
        IncludeActionItems = includeActionItems;
        IncludeDecisions = includeDecisions;
        Language = language;
    }
}
