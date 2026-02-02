using FluentValidation;

namespace SpeechToText.Features.TranscribeAudio.V1;

public class TranscribeAudioCommandValidator : AbstractValidator<TranscribeAudioCommand>
{
    public TranscribeAudioCommandValidator()
    {
        RuleFor(x => x.AudioUrl).NotEmpty();
        RuleFor(x => x.Language).NotEmpty();
    }
}
