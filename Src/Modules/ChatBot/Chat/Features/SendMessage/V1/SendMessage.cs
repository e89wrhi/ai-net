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

namespace ChatBot.Features.SendMessage.V1;

public record SendMessageCommand() : ICommand<SendMessageCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record SendMessageCommandResponse(Guid Id);

public record SendMessageRequest();

public record SendMessageRequestResponse(Guid Id);

public class SendMessageEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/chat", async (SendMessageRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<SendMessageCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<SendMessageRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("SendMessage")
            .WithApiVersionSet(builder.NewApiVersionSet("Chat").Build())
            .Produces<SendMessageRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Send Message")
            .WithDescription("Send Message")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
    }
}

internal class SendMessageHandler : IRequestHandler<SendMessageCommand, SendMessageCommandResponse>
{
    private readonly ChatDbContext _dbContext;

    public SendMessageHandler(ChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SendMessageCommandResponse> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        await _dbContext.SaveChangesAsync(cancellationToken);
        return new SendMessageCommandResponse(item.Id);
    }
}
