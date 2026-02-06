using FluentValidation;

namespace SpeechToText.Features.StreamTranscribeAudio.V1;

public class StreamTranscribeAudioCommandValidator : AbstractValidator<StreamTranscribeAudioCommand>
{
    public StreamTranscribeAudioCommandValidator()
    {
        RuleFor(x => x.AudioUrl).NotEmpty();
        RuleFor(x => x.Language).NotEmpty();
        RuleFor(x => x.ModelId).MaximumLength(250).When(x => x.ModelId != null);

    }
}
