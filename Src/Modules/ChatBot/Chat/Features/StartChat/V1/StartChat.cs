using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using ChatBot.Data;
using ChatBot.Models;
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

namespace ChatBot.Features.StartChat.V1;

public record StartChatCommand(Guid UserId, string Title, string AiModelId) : ICommand<StartChatCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record StartChatCommandResponse(Guid Id);

public record StartChatRequest(Guid UserId, string Title, string AiModelId);

public record StartChatRequestResponse(Guid Id);

public class StartChatEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/chat", async (StartChatRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<StartChatCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<StartChatRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StartChat")
            .WithApiVersionSet(builder.NewApiVersionSet("Chat").Build())
            .Produces<StartChatRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Start Chat")
            .WithDescription("Start Chat")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class StartChatCommandValidator : AbstractValidator<StartChatCommand>
{
    public StartChatCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.AiModelId).NotEmpty();
    }
}

internal class StartChatHandler : IRequestHandler<StartChatCommand, StartChatCommandResponse>
{
    private readonly ChatDbContext _dbContext;

    public StartChatHandler(ChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StartChatCommandResponse> Handle(StartChatCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var chat = ChatModel.Create(
            SessionId.Of(NewId.NextGuid()),
            UserId.Of(request.UserId),
            request.Title,
            request.AiModelId);

        await _dbContext.Chats.AddAsync(chat, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new StartChatCommandResponse(chat.Id.Value);
    }
}

