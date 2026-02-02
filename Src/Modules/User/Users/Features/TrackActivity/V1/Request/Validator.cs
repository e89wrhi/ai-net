using FluentValidation;

namespace User.Features.TrackActivity.V1;


public class TrackActivityCommandValidator : AbstractValidator<TrackActivityCommand>
{
    public TrackActivityCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Action).NotEmpty();
    }
}