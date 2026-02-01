using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Google.Protobuf.WellKnownTypes;
using Sentiment;

namespace Sentiment.GrpcServer.Services;

public class SentimentGrpcService : Sentiment.SentimentGrpcService.SentimentGrpcServiceBase
{
    private readonly IMediator _mediator;

    public SentimentGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<StartSentimentResponse> StartSentiment(StartSentimentRequest request, ServerCallContext context)
    {
        var cmd = new Sentiment.Features.StartSentiment.V1.StartSentimentCommand(
            Guid.Parse(request.UserId),
            request.Title,
            request.AiModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new StartSentimentResponse
        {
            SessionId = result.Id.ToString()
        };
    }

    public override async Task<DeleteSentimentResponse> DeleteSentiment(DeleteSentimentRequest request, ServerCallContext context)
    {
        var cmd = new Sentiment.Features.DeleteSentiment.V1.DeleteSentimentCommand(Guid.Parse(request.SessionId));
        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new DeleteSentimentResponse
        {
            SessionId = result.Id.ToString()
        };
    }

    public override async Task<GetSentimentHistoryResponse> GetSentimentHistory(GetSentimentHistoryRequest request, ServerCallContext context)
    {
        var query = new Sentiment.Features.GetSentimentHistory.V1.GetSentimentHistory(Guid.Parse(request.UserId));
        var result = await _mediator.Send(query, context.CancellationToken);

        var response = new GetSentimentHistoryResponse();

        foreach (var dto in result.SentimentDtos)
        {
            var summary = new SentimentSummary
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

            // Sentiments are not included in SentimentDto currently; leave messages empty.
            response.Sentiments.Add(summary);
        }

        return response;
    }
}
