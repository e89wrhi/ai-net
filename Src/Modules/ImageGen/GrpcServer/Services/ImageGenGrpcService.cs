using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Google.Protobuf.WellKnownTypes;
using ImageGen;

namespace ImageGen.GrpcServer.Services;

public class ImageGenGrpcService : ImageGen.ImageGenGrpcService.ImageGenGrpcServiceBase
{
    private readonly IMediator _mediator;

    public ImageGenGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<StartImageGenResponse> StartImageGen(StartImageGenRequest request, ServerCallContext context)
    {
        var cmd = new ImageGen.Features.StartImageGen.V1.StartImageGenCommand(
            Guid.Parse(request.UserId),
            request.Title,
            request.AiModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new StartImageGenResponse
        {
            SessionId = result.Id.ToString()
        };
    }

    public override async Task<DeleteImageGenResponse> DeleteImageGen(DeleteImageGenRequest request, ServerCallContext context)
    {
        var cmd = new ImageGen.Features.DeleteImageGen.V1.DeleteImageGenCommand(Guid.Parse(request.SessionId));
        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new DeleteImageGenResponse
        {
            SessionId = result.Id.ToString()
        };
    }

    public override async Task<GetImageGenHistoryResponse> GetImageGenHistory(GetImageGenHistoryRequest request, ServerCallContext context)
    {
        var query = new ImageGen.Features.GetImageGenHistory.V1.GetImageGenHistory(Guid.Parse(request.UserId));
        var result = await _mediator.Send(query, context.CancellationToken);

        var response = new GetImageGenHistoryResponse();

        foreach (var dto in result.ImageGenDtos)
        {
            var summary = new ImageGenSummary
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

            // ImageGens are not included in ImageGenDto currently; leave messages empty.
            response.ImageGens.Add(summary);
        }

        return response;
    }
}
