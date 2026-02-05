using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using TextToSpeech.GrpcServer.Protos;

using Protos = TextToSpeech.GrpcServer.Protos;

namespace TextToSpeech.GrpcServer.Services;

public class TextToSpeechGrpcService : Protos.TextToSpeechGrpcService.TextToSpeechGrpcServiceBase
{
    private readonly IMediator _mediator;

    public TextToSpeechGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<SynthesizeSpeechResponse> SynthesizeSpeech(SynthesizeSpeechRequest request, ServerCallContext context)
    {
        var cmd = new TextToSpeech.Features.SynthesizeSpeech.V1.SynthesizeSpeechCommand(
            Guid.Parse(request.UserId),
            request.Text,
            (TextToSpeech.Enums.VoiceType)(int)request.Voice,
            (TextToSpeech.Enums.SpeechSpeed)(int)request.Speed,
            request.Language,
            request.ModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new SynthesizeSpeechResponse
        {
            SessionId = result.SessionId.ToString(),
            ResultId = result.ResultId.ToString(),
            AudioUrl = result.AudioUrl,
            ModelId = result.ModelId,
            ProviderName = result.ProviderName ?? string.Empty
        };
    }

    public override async Task<GenerateAudioResponse> GenerateAudio(GenerateAudioRequest request, ServerCallContext context)
    {
        var cmd = new TextToSpeech.Features.GenerateAudio.V1.GenerateAudioCommand(
            Guid.Parse(request.UserId),
            request.Text,
            (TextToSpeech.Enums.VoiceType)(int)request.Voice,
            (TextToSpeech.Enums.SpeechSpeed)(int)request.Speed,
            request.ModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new GenerateAudioResponse
        {
            SessionId = result.SessionId.ToString(),
            AudioUrl = result.AudioUrl,
            ModelId = result.ModelId,
            ProviderName = result.ProviderName ?? string.Empty
        };
    }
}

