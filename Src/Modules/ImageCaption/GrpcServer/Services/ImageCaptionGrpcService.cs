using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using ImageCaption.Features.AnalyzeImage.V1;
using ImageCaption.Features.CaptionImage.V1;
using ImageCaption.GrpcServer.Protos;

using Protos = ImageCaption.GrpcServer.Protos;

namespace ImageCaption.GrpcServer.Services;

public class ImageCaptionGrpcService : Protos.ImageCaptionGrpcService.ImageCaptionGrpcServiceBase
{
    private readonly IMediator _mediator;

    public ImageCaptionGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<CaptionImageResponse> CaptionImage(CaptionImageRequest request, ServerCallContext context)
    {
        var cmd = new ImageCaptionCommand(
            Guid.Parse(request.UserId),
            request.ImageUrlOrBase64,
            (ImageCaption.Enums.CaptionDetailLevel)(int)request.Level,
            request.ModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new CaptionImageResponse
        {
            SessionId = result.SessionId.ToString(),
            ResultId = result.ResultId.ToString(),
            Caption = result.Caption,
            ModelId = result.ModelId,
            ProviderName = result.ProviderName ?? string.Empty
        };
    }

    public override async Task<AnalyzeImageResponse> AnalyzeImage(AnalyzeImageRequest request, ServerCallContext context)
    {
        var cmd = new AnalyzeImageCommand(
            Guid.Parse(request.UserId),
            request.ImageUrlOrBase64,
            (ImageCaption.Enums.CaptionDetailLevel)(int)request.Level,
            request.ModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new AnalyzeImageResponse
        {
            SessionId = result.SessionId.ToString(),
            ResultId = result.ResultId.ToString(),
            Analysis = result.Analysis,
            ModelId = result.ModelId,
            ProviderName = result.ProviderName ?? string.Empty
        };
    }
}

