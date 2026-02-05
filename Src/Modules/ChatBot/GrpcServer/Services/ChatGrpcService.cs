using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Google.Protobuf.WellKnownTypes;
using ChatBot.GrpcServer.Protos;

using Protos = ChatBot.GrpcServer.Protos;

namespace ChatBot.GrpcServer.Services;

public class ChatGrpcService : Protos.ChatGrpcService.ChatGrpcServiceBase
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

    public override async Task<SendMessageResponse> SendMessage(SendMessageRequest request, ServerCallContext context)
    {
        var cmd = new ChatBot.Features.SendMessage.V1.SendMessageCommand(
            Guid.Parse(request.SessionId),
            request.Content);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new SendMessageResponse
        {
            MessageId = result.MessageId.ToString()
        };
    }

    public override async Task<GenerateResponseResponse> GenerateResponse(GenerateResponseRequest request, ServerCallContext context)
    {
        var cmd = new ChatBot.Features.GenerateResponse.V1.GenerateAiResponseCommand(
            Guid.Parse(request.SessionId),
            request.ModelId);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new GenerateResponseResponse
        {
            MessageId = result.MessageId.ToString(),
            Content = result.Content,
            ModelId = result.ModelId,
            ProviderName = result.ProviderName ?? string.Empty
        };
    }

    public override async Task StreamResponse(StreamResponseRequest request, IServerStreamWriter<StreamResponseResponse> responseStream, ServerCallContext context)
    {
        var cmd = new ChatBot.Features.StreamResponse.V1.StreamAiResponseCommand(
            Guid.Parse(request.SessionId));

        var stream = _mediator.CreateStream(cmd, context.CancellationToken);

        await foreach (var item in stream)
        {
            await responseStream.WriteAsync(new StreamResponseResponse
            {
                Text = item
            });
        }
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

            if (dto.LastSentAt != default)
            {
                var utc = DateTime.SpecifyKind(dto.LastSentAt.ToUniversalTime(), DateTimeKind.Utc);
                summary.LastSentAt = Timestamp.FromDateTime(utc);
            }

            response.Chats.Add(summary);
        }

        return response;
    }

    public override async Task<GetChatByIdResponse> GetChatById(GetChatByIdRequest request, ServerCallContext context)
    {
        var query = new ChatBot.Features.GetChatById.V1.GetChatByIdQuery(Guid.Parse(request.SessionId), Guid.Parse(request.UserId));
        var result = await _mediator.Send(query, context.CancellationToken);

        var dto = result.Chat;
        var summary = new ChatSummary
        {
            Id = dto.Id.ToString(),
            Title = dto.Title,
            Summary = dto.Summary,
            AiModelId = dto.AiModelId,
            SessionStatus = dto.SessionStatus,
            TotalTokens = dto.TotalTokens
        };

        if (dto.LastSentAt != default)
        {
            var utc = DateTime.SpecifyKind(dto.LastSentAt.ToUniversalTime(), DateTimeKind.Utc);
            summary.LastSentAt = Timestamp.FromDateTime(utc);
        }

        return new GetChatByIdResponse { Chat = summary };
    }

    public override async Task<UpdateChatResponse> UpdateChat(UpdateChatRequest request, ServerCallContext context)
    {
        var cmd = new ChatBot.Features.UpdateChat.V1.UpdateChatCommand(
            Guid.Parse(request.SessionId),
            Guid.Parse(request.UserId),
            request.Title);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new UpdateChatResponse { Success = result.Success };
    }
}
