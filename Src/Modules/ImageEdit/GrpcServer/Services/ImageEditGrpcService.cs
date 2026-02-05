using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using ImageEdit.Features.EnhanceImage.V1;
using ImageEdit.Features.RemoveBackground.V1;
using ImageEdit.GrpcServer.Protos;

using Protos = ImageEdit.GrpcServer.Protos;

namespace ImageEdit.GrpcServer.Services;

public class ImageEditGrpcService : Protos.ImageEditGrpcService.ImageEditGrpcServiceBase
{
    private readonly IMediator _mediator;

    public ImageEditGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<EnhanceImageResponse> EnhanceImage(EnhanceImageRequest request, ServerCallContext context)
    {
        var cmd = new AIEnhanceImageCommand(
            Guid.Parse(request.UserId),
            request.ImageUrlOrBase64,
            request.Prompt,
            (ImageEdit.Enums.ImageEditQuality)(int)request.Quality,
            (ImageEdit.Enums.ImageFormat)(int)request.Format,
             request.ModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new EnhanceImageResponse
        {
            SessionId = result.SessionId.ToString(),
            ResultId = result.ResultId.ToString(),
            ResultImageUrl = result.ResultImageUrl,
            ModelId = result.ModelId,
            ProviderName = result.ProviderName ?? string.Empty
        };
    }

    public override async Task<RemoveBackgroundResponse> RemoveBackground(RemoveBackgroundRequest request, ServerCallContext context)
    {
        var cmd = new RemoveBackgroundCommand(
            Guid.Parse(request.UserId),
            request.ImageUrlOrBase64,
            (ImageEdit.Enums.ImageEditQuality)(int)request.Quality,
            (ImageEdit.Enums.ImageFormat)(int)request.Format,
             request.ModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new RemoveBackgroundResponse
        {
            SessionId = result.SessionId.ToString(),
            ResultId = result.ResultId.ToString(),
            ResultImageUrl = result.ResultImageUrl,
            ModelId = result.ModelId,
            ProviderName = result.ProviderName ?? string.Empty
        };
    }
}

