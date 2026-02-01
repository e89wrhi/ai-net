using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Google.Protobuf.WellKnownTypes;
using AutoComplete;

namespace AutoComplete.GrpcServer.Services;

public class AutoCompleteGrpcService : AutoComplete.AutoCompleteGrpcService.AutoCompleteGrpcServiceBase
{
    private readonly IMediator _mediator;

    public AutoCompleteGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<StartAutoCompleteResponse> StartAutoComplete(StartAutoCompleteRequest request, ServerCallContext context)
    {
        var cmd = new AutoComplete.Features.StartAutoComplete.V1.StartAutoCompleteCommand(
            Guid.Parse(request.UserId),
            request.Title,
            request.AiModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new StartAutoCompleteResponse
        {
            SessionId = result.Id.ToString()
        };
    }

    public override async Task<DeleteAutoCompleteResponse> DeleteAutoComplete(DeleteAutoCompleteRequest request, ServerCallContext context)
    {
        var cmd = new AutoComplete.Features.DeleteAutoComplete.V1.DeleteAutoCompleteCommand(Guid.Parse(request.SessionId));
        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new DeleteAutoCompleteResponse
        {
            SessionId = result.Id.ToString()
        };
    }

    public override async Task<GetAutoCompleteHistoryResponse> GetAutoCompleteHistory(GetAutoCompleteHistoryRequest request, ServerCallContext context)
    {
        var query = new AutoComplete.Features.GetAutoCompleteHistory.V1.GetAutoCompleteHistory(Guid.Parse(request.UserId));
        var result = await _mediator.Send(query, context.CancellationToken);

        var response = new GetAutoCompleteHistoryResponse();

        foreach (var dto in result.AutoCompleteDtos)
        {
            var summary = new AutoCompleteSummary
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

            // AutoCompletes are not included in AutoCompleteDto currently; leave messages empty.
            response.AutoCompletes.Add(summary);
        }

        return response;
    }
}
