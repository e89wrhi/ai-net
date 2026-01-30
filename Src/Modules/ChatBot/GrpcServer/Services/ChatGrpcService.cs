using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Google.Protobuf.WellKnownTypes;
using ChatBot;

namespace ChatBot.GrpcServer.Services;

public class ChatGrpcService : ChatBot.ChatGrpcService.ChatGrpcServiceBase
{
    private readonly IMediator _mediator;

    public ChatGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<StartChatResponse> StartChat(StartChatRequest request, ServerCallContext context)
    {
        var cmd = new ChatBot.Features.StartChat.V1.StartChatCommand(
            Guid.Parse(request.UserId),
            request.Title,
            request.AiModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new StartChatResponse
        {
            SessionId = result.Id.ToString()
        };
    }

    public override async Task<DeleteChatResponse> DeleteChat(DeleteChatRequest request, ServerCallContext context)
    {
        var cmd = new ChatBot.Features.DeleteChat.V1.DeleteChatCommand(Guid.Parse(request.SessionId));
        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new DeleteChatResponse
        {
            SessionId = result.Id.ToString()
        };
    }

    public override async Task<GetChatHistoryResponse> GetChatHistory(GetChatHistoryRequest request, ServerCallContext context)
    {
        var query = new ChatBot.Features.GetChatHistory.V1.GetChatHistory(Guid.Parse(request.UserId));
        var result = await _mediator.Send(query, context.CancellationToken);

        var response = new GetChatHistoryResponse();

        foreach (var dto in result.ChatDtos)
        {
            var summary = new ChatSummary
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

            // Messages are not included in ChatDto currently; leave messages empty.
            response.Chats.Add(summary);
        }

        return response;
    }
}
