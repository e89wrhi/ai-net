using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Sentiment.GrpcServer.Protos;

using Protos = Sentiment.GrpcServer.Protos;

namespace Sentiment.GrpcServer.Services;

public class SentimentGrpcService : Protos.SentimentGrpcService.SentimentGrpcServiceBase
{
    private readonly IMediator _mediator;

    public SentimentGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<AnalyzeSentimentResponse> AnalyzeSentiment(AnalyzeSentimentRequest request, ServerCallContext context)
    {
        var cmd = new Sentiment.Features.AnalyzeSentiment.V1.AnalyzeSentimentCommand(
            Guid.Parse(request.UserId),
            request.Text,
            request.ModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new AnalyzeSentimentResponse
        {
            SessionId = result.SessionId.ToString(),
            ResultId = result.ResultId.ToString(),
            Sentiment = result.Sentiment,
            Score = result.Score,
            ModelId = result.ModelId,
            ProviderName = result.ProviderName ?? string.Empty
        };
    }

    public override async Task<AnalyzeSentimentDetailedResponse> AnalyzeSentimentDetailed(AnalyzeSentimentDetailedRequest request, ServerCallContext context)
    {
        var cmd = new Sentiment.Features.AnalyzeSentimentDetailed.V1.AnalyzeSentimentDetailedCommand(
            Guid.Parse(request.UserId),
            request.Text,
            request.Language,
            (Sentiment.Enums.SentimentDetailLevel)(int)request.DetailLevel,
            request.ModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new AnalyzeSentimentDetailedResponse
        {
            SessionId = result.SessionId.ToString(),
            ResultId = result.ResultId.ToString(),
            Sentiment = result.Sentiment,
            Score = result.Score,
            Explanation = result.Explanation,
            ModelId = result.ModelId,
            ProviderName = result.ProviderName ?? string.Empty
        };
    }
}

