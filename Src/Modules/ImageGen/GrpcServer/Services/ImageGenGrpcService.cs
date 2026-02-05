using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using ImageGen.GrpcServer.Protos;

using Protos = ImageGen.GrpcServer.Protos;

namespace ImageGen.GrpcServer.Services;

public class ImageGenGrpcService : Protos.ImageGenGrpcService.ImageGenGrpcServiceBase
{
    private readonly IMediator _mediator;

    public ImageGenGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<GenerateImageResponse> GenerateImage(GenerateImageRequest request, ServerCallContext context)
    {
        var cmd = new ImageGen.Features.GenerateImage.V1.GenerateImageCommand(
            Guid.Parse(request.UserId),
            request.Prompt,
            (ImageGen.Enums.ImageSize)(int)request.Size,
            (ImageGen.Enums.ImageStyle)(int)request.Style,
            (ImageGen.Enums.ImageQuality)(int)request.Quality,
            request.ModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new GenerateImageResponse
        {
            SessionId = result.SessionId.ToString(),
            ResultId = result.ResultId.ToString(),
            ImageUrl = result.ImageUrl,
            ModelId = result.ModelId,
            ProviderName = result.ProviderName ?? string.Empty
        };
    }

    public override async Task<ReGenerateImageResponse> ReGenerateImage(ReGenerateImageRequest request, ServerCallContext context)
    {
        var cmd = new ImageGen.Features.ReGenerateImage.V1.ReGenerateImageCommand(
            Guid.Parse(request.UserId),
            Guid.Parse(request.SessionId),
            request.Instruction,
            request.ModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new ReGenerateImageResponse
        {
            ResultId = result.ResultId.ToString(),
            ImageUrl = result.ImageUrl,
            ModelId = result.ModelId,
            ProviderName = result.ProviderName ?? string.Empty
        };
    }
}

