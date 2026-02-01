using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using CodeDebug.Data;
using CodeDebug.Exceptions;
using CodeDebug.Models;
using CodeDebug.ValueObjects;
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

namespace CodeDebug.Features.SendCodeDebug.V1;

public record SendCodeDebugCommand(Guid SessionId, string Content, string Sender, int TokenUsed) : ICommand<SendCodeDebugCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record SendCodeDebugCommandResponse(Guid Id);

public record SendCodeDebugRequest(Guid SessionId, string Content, string Sender, int TokenUsed);

public record SendCodeDebugRequestResponse(Guid Id);

public class SendCodeDebugEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/codedebug/send-message", async (SendCodeDebugRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<SendCodeDebugCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<SendCodeDebugRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("SendCodeDebug")
            .WithApiVersionSet(builder.NewApiVersionSet("CodeDebug").Build())
            .Produces<SendCodeDebugRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Send CodeDebug")
            .WithDescription("Send CodeDebug")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class SendCodeDebugCommandValidator : AbstractValidator<SendCodeDebugCommand>
{
    public SendCodeDebugCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
        RuleFor(x => x.Content).NotEmpty();
    }
}

internal class SendCodeDebugHandler : IRequestHandler<SendCodeDebugCommand, SendCodeDebugCommandResponse>
{
    private readonly CodeDebugDbContext _dbContext;

    public SendCodeDebugHandler(CodeDebugDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SendCodeDebugCommandResponse> Handle(SendCodeDebugCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var codedebug = await _dbContext.CodeDebugs.FindAsync(new object[] { SessionId.Of(request.SessionId) }, cancellationToken);

        if (codedebug == null)
        {
            throw new CodeDebugNotFoundException(request.SessionId);
        }

        var message = CodeDebugModel.Create(
            CodeDebugId.Of(NewId.NextGuid()),
            codedebug.Id,
            CodeDebugSender.Of(request.Sender),
            CodeDebugContent.Of(request.Content),
            TokenUsed.Of(request.TokenUsed));

        codedebug.AddCodeDebug(message);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SendCodeDebugCommandResponse(message.Id.Value);
    }
}

