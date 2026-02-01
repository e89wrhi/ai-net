using Summary.Enums;
using Summary.Exceptions;

namespace Summary.ValueObjects;

public record TextSummaryConfiguration
{
    public SummaryDetailLevel DetailLevel { get; }
    public LanguageCode Language { get; }

    public TextSummaryConfiguration(SummaryDetailLevel detailLevel, LanguageCode language)
    {
        DetailLevel = detailLevel;
        Language = language;
    }
}
