using FluentValidation;

namespace Sentiment.Features.AnalyzeSentimentDetailed.V1;

public class AnalyzeSentimentDetailedCommandValidator : AbstractValidator<AnalyzeSentimentDetailedCommand>
{
    public AnalyzeSentimentDetailedCommandValidator()
    {
        RuleFor(x => x.Text).NotEmpty();
    }
}
