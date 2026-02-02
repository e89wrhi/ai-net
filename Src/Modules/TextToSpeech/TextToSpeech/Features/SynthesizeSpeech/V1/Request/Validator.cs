using FluentValidation;

namespace TextToSpeech.Features.SynthesizeSpeech.V1;

public class SynthesizeSpeechCommandValidator : AbstractValidator<SynthesizeSpeechCommand>
{
    public SynthesizeSpeechCommandValidator()
    {
        RuleFor(x => x.Text).NotEmpty();
        RuleFor(x => x.Voice).NotEmpty();
        RuleFor(x => x.Speed).NotEmpty();
        RuleFor(x => x.Language).NotEmpty();
    }
}

