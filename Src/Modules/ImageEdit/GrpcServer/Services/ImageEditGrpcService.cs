using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Google.Protobuf.WellKnownTypes;
using ImageEdit;

namespace ImageEdit.GrpcServer.Services;

public class ImageEditGrpcService : ImageEdit.ImageEditGrpcService.ImageEditGrpcServiceBase
{
    private readonly IMediator _mediator;

    public ImageEditGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<StartImageEditResponse> StartImageEdit(StartImageEditRequest request, ServerCallContext context)
    {
        var cmd = new ImageEdit.Features.StartImageEdit.V1.StartImageEditCommand(
            Guid.Parse(request.UserId),
            request.Title,
            request.AiModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new StartImageEditResponse
        {
            SessionId = result.Id.ToString()
        };
    }

    public override async Task<DeleteImageEditResponse> DeleteImageEdit(DeleteImageEditRequest request, ServerCallContext context)
    {
        var cmd = new ImageEdit.Features.DeleteImageEdit.V1.DeleteImageEditCommand(Guid.Parse(request.SessionId));
        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new DeleteImageEditResponse
        {
            SessionId = result.Id.ToString()
        };
    }

    public override async Task<GetImageEditHistoryResponse> GetImageEditHistory(GetImageEditHistoryRequest request, ServerCallContext context)
    {
        var query = new ImageEdit.Features.GetImageEditHistory.V1.GetImageEditHistory(Guid.Parse(request.UserId));
        var result = await _mediator.Send(query, context.CancellationToken);

        var response = new GetImageEditHistoryResponse();

        foreach (var dto in result.ImageEditDtos)
        {
            var summary = new ImageEditSummary
            {
                Id = dto.Id.ToString(),
                Title = dto.Title,
                Summary = dto.Summary,
                AiModelId = dto.AiModelId,
                SessionStatus = dto.SessionStatus,
                TotalTokens = dto.TotalTokens
            };

            // Map last sent timestamp if available
            if (dto.LastSentAt != default)
            {
                var utc = DateTime.SpecifyKind(dto.LastSentAt.ToUniversalTime(), DateTimeKind.Utc);
                summary.LastSentAt = Timestamp.FromDateTime(utc);
            }

            // ImageEdits are not included in ImageEditDto currently; leave messages empty.
            response.ImageEdits.Add(summary);
        }

        return response;
    }
}
