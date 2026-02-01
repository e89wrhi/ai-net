using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ImageCaption.GrpcServer.Services;

public class ImageCaptionGrpcService : ImageCaption.ImageGrpcService.ImageGrpcServiceBase
{
    private readonly IMediator _mediator;
    private readonly IServiceProvider _serviceProvider;

    public ImageCaptionGrpcService(IMediator mediator, IServiceProvider serviceProvider)
    {
        _mediator = mediator;
        _serviceProvider = serviceProvider;
    }

    public override async Task<GenerateCaptionResponse> GenerateCaption(GenerateCaptionRequest request, ServerCallContext context)
    {
        var cmd = new ImageCaption.Features.GenerateCaption.V1.GenerateCaptionCommand(
            Guid.Parse(request.ImageId),
            request.CaptionText,
            request.Confidence,
            request.Language);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new GenerateCaptionResponse { Id = result.Id.ToString() };
    }

    public override async Task<GetImageCaptionsResponse> GetImageCaptions(GetImageCaptionsRequest request, ServerCallContext context)
    {
        // Read side likely lives in a "ImageCaption.Data.ImageReadDbContext" (Mongo). Resolve if present.
        var response = new GetImageCaptionsResponse();

        try
        {
            var readDb = _serviceProvider.GetService<ImageCaption.Data.ImageCaptionReadDbContext>();
            if (readDb != null)
            {
                var image = await readDb.Image.AsQueryable()
                    .FirstOrDefaultAsync(x => x.Id == Guid.Parse(request.ImageId), context.CancellationToken);

                if (image != null)
                {
                    foreach (var c in image.Captions)
                    {
                        var caption = new Caption
                        {
                            Id = c.Id.ToString(),
                            Text = c.Text ?? string.Empty,
                            Confidence = c.ConfidenceScore,
                            Language = c.Language ?? string.Empty
                        };

                        if (c.CreatedAt != default)
                        {
                            var utc = DateTime.SpecifyKind(c.CreatedAt.ToUniversalTime(), DateTimeKind.Utc);
                            caption.CreatedAt = Timestamp.FromDateTime(utc);
                        }

                        response.Captions.Add(caption);
                    }
                }
            }
        }
        catch
        {
            // swallow - return empty list when read-side is unavailable
        }

        return response;
    }
}
