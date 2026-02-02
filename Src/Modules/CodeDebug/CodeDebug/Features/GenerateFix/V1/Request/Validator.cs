using FluentValidation;

namespace CodeDebug.Features.GenerateFix.V1;

public class GenerateFixCommandValidator : AbstractValidator<GenerateFixCommand>
{
    public GenerateFixCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
        RuleFor(x => x.ReportId).NotEmpty();
    }
}
