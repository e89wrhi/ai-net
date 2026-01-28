using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using ChatBot.Data;
using ChatBot.ValueObjects;
using Duende.IdentityServer.EntityFramework.Entities;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MassTransit;
using MassTransit.Contracts;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ChatBot.Features.DeleteChat.V1;

public record DeleteChatCommand(Guid SessionId) : ICommand<DeleteChatCommandResponse>;

public record DeleteChatCommandResponse(Guid Id);

public record DeleteChatRequest(Guid SessionId);

public record DeleteChatRequestResponse(Guid Id);

public class DeleteChatEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete($"{EndpointConfig.BaseApiPath}/chat/{{sessionId}}", async (Guid sessionId,
                IMediator mediator, MapsterMapper.IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new DeleteChatCommand(sessionId), cancellationToken);

            var response = result.Adapt<DeleteChatRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("DeleteChat")
            .WithApiVersionSet(builder.NewApiVersionSet("Chat").Build())
            .Produces<DeleteChatRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Delete Chat")
            .WithDescription("Delete Chat")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class DeleteChatCommandValidator : AbstractValidator<DeleteChatCommand>
{
    public DeleteChatCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
    }
}

internal class DeleteChatHandler : IRequestHandler<DeleteChatCommand, DeleteChatCommandResponse>
{
    private readonly ChatDbContext _dbContext;

    public DeleteChatHandler(ChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeleteChatCommandResponse> Handle(DeleteChatCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var chat = await _dbContext.Chats.FindAsync(new object[] { SessionId.Of(request.SessionId) }, cancellationToken);

        if (chat == null)
        {
            throw new ChatNotFoundException(request.SessionId);
        }

        chat.Delete();
        _dbContext.Chats.Remove(chat);

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new DeleteChatCommandResponse(chat.Id.Value);
    }
}

