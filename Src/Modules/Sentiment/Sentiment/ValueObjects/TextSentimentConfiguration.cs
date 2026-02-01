using Sentiment.Enums;
using Sentiment.Exceptions;

namespace Sentiment.ValueObjects;

public record TextSentimentConfiguration
{
    public SentimentDetailLevel DetailLevel { get; }
    public LanguageCode Language { get; }

    public TextSentimentConfiguration(SentimentDetailLevel detailLevel, LanguageCode language)
    {
        DetailLevel = detailLevel;
        Language = language;
    }
}

