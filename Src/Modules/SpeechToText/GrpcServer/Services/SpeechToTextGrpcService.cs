using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using SpeechToText.GrpcServer.Protos;

using Protos = SpeechToText.GrpcServer.Protos;

namespace SpeechToText.GrpcServer.Services;

public class SpeechToTextGrpcService : Protos.SpeechToTextGrpcService.SpeechToTextGrpcServiceBase
{
    private readonly IMediator _mediator;

    public SpeechToTextGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<TranscribeAudioResponse> TranscribeAudio(TranscribeAudioRequest request, ServerCallContext context)
    {
        var cmd = new SpeechToText.Features.TranscribeAudio.V1.TranscribeAudioCommand(
            Guid.Parse(request.UserId),
            request.AudioUrl,
            request.Language,
            request.IncludePunctuation,
            (SpeechToText.Enums.SpeechToTextDetailLevel)(int)request.DetailLevel,
            request.ModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new TranscribeAudioResponse
        {
            SessionId = result.SessionId.ToString(),
            ResultId = result.ResultId.ToString(),
            Transcript = result.Transcript,
            ModelId = result.ModelId,
            ProviderName = result.ProviderName ?? string.Empty
        };
    }

    public override async Task StreamTranscribeAudio(StreamTranscribeAudioRequest request, IServerStreamWriter<StreamTranscribeAudioResponse> responseStream, ServerCallContext context)
    {
        var cmd = new SpeechToText.Features.StreamTranscribeAudio.V1.StreamTranscribeAudioCommand(
            Guid.Parse(request.UserId),
            request.AudioUrl,
            request.Language,
            request.IncludePunctuation,
            (SpeechToText.Enums.SpeechToTextDetailLevel)(int)request.DetailLevel,
            request.ModelId);

        var stream = _mediator.CreateStream(cmd, context.CancellationToken);

        await foreach (var item in stream)
        {
            await responseStream.WriteAsync(new StreamTranscribeAudioResponse
            {
                Text = item
            });
        }
    }
}
