using FluentValidation;

namespace TextToSpeech.Features.GenerateAudio.V1;

public class GenerateAudioWithAICommandValidator : AbstractValidator<GenerateAudioWithAICommand>
{
    public GenerateAudioWithAICommandValidator()
    {
        RuleFor(x => x.Text).NotEmpty();
        RuleFor(x => x.Voice).NotEmpty();
    }
}
