using FluentValidation;

namespace Meeting.Features.StreamMeetingAnalysis.V1;

public class StreamMeetingAnalysisCommandValidator : AbstractValidator<StreamMeetingAnalysisCommand>
{
    public StreamMeetingAnalysisCommandValidator()
    {
        RuleFor(x => x.Transcript).NotEmpty();
        RuleFor(x => x.ModelId).MaximumLength(250).When(x => x.ModelId != null);

    }
}
