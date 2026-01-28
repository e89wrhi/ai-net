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

public record DeleteChatCommand(SessionId id) : ICommand<DeleteChatCommandResponse>
{
}

public record DeleteChatCommandResponse(Guid Id);

public record DeleteChatRequest();

public record DeleteChatRequestResponse(Guid Id);

public class DeleteChatEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/chat", async (DeleteChatRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<DeleteChatCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

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
         
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new DeleteChatCommandResponse(item.Id);
    }
}
