using FluentValidation;

namespace Sentiment.Features.AnalyzeSentimentWithAI.V1;

public class AnalyzeSentimentWithAICommandValidator : AbstractValidator<AnalyzeSentimentWithAICommand>
{
    public AnalyzeSentimentWithAICommandValidator()
    {
        RuleFor(x => x.Text).NotEmpty();
    }
}
