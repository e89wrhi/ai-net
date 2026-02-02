using FluentValidation;

namespace Meeting.Features.StreamMeetingAnalysis.V1;

public class StreamMeetingAnalysisCommandValidator : AbstractValidator<StreamMeetingAnalysisCommand>
{
    public StreamMeetingAnalysisCommandValidator()
    {
        RuleFor(x => x.Transcript).NotEmpty();
    }
}
