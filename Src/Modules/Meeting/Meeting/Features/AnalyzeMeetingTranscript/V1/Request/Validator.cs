using FluentValidation;

namespace Meeting.Features.AnalyzeMeetingTranscript.V1;


public class AnalyzeMeetingTranscriptCommandValidator : AbstractValidator<AnalyzeMeetingTranscriptCommand>
{
    public AnalyzeMeetingTranscriptCommandValidator()
    {
        RuleFor(x => x.Transcript).NotEmpty();
    }
}
