using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Translate.GrpcServer.Protos;

using Protos = Translate.GrpcServer.Protos;

namespace Translate.GrpcServer.Services;

public class TranslateGrpcService : Protos.TranslateGrpcService.TranslateGrpcServiceBase
{
    private readonly IMediator _mediator;

    public TranslateGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<TranslateTextResponse> TranslateText(TranslateTextRequest request, ServerCallContext context)
    {
        var cmd = new Translate.Features.TranslateText.V1.TranslateTextCommand(
            Guid.Parse(request.UserId),
            request.Text,
            request.SourceLanguage,
            request.TargetLanguage,
            (Translate.Enums.TranslationDetailLevel)(int)request.DetailLevel,
            request.ModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new TranslateTextResponse
        {
            SessionId = result.SessionId.ToString(),
            ResultId = result.ResultId.ToString(),
            TranslatedText = result.TranslatedText,
            ModelId = result.ModelId,
            ProviderName = result.ProviderName ?? string.Empty
        };
    }

    public override async Task StreamTranslateText(StreamTranslateTextRequest request, IServerStreamWriter<StreamTranslateTextResponse> responseStream, ServerCallContext context)
    {
        var cmd = new Translate.Features.StreamTranslateText.V1.StreamTranslateTextCommand(
            Guid.Parse(request.UserId),
            request.Text,
            request.SourceLanguage,
            request.TargetLanguage,
            (Translate.Enums.TranslationDetailLevel)(int)request.DetailLevel,
            request.ModelId);

        var stream = _mediator.CreateStream(cmd, context.CancellationToken);

        await foreach (var item in stream)
        {
            await responseStream.WriteAsync(new StreamTranslateTextResponse
            {
                Text = item
            });
        }
    }

    public override async Task<DetectLanguageResponse> DetectLanguage(DetectLanguageRequest request, ServerCallContext context)
    {
        var cmd = new Translate.Features.DetectLanguage.V1.DetectLanguageCommand(
            Guid.Parse(request.UserId),
            request.Text,
            request.ModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new DetectLanguageResponse
        {
            DetectedLanguageCode = result.DetectedLanguageCode,
            Confidence = result.Confidence,
            ModelId = result.ModelId,
            ProviderName = result.ProviderName ?? string.Empty
        };
    }
}

