using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using CodeDebug.Data;
using CodeDebug.Exceptions;
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

namespace CodeDebug.Features.DeleteCodeDebug.V1;

public record DeleteCodeDebugCommand(Guid SessionId) : ICommand<DeleteCodeDebugCommandResponse>;

public record DeleteCodeDebugCommandResponse(Guid Id);

public record DeleteCodeDebugRequest(Guid SessionId);

public record DeleteCodeDebugRequestResponse(Guid Id);

public class DeleteCodeDebugEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete($"{EndpointConfig.BaseApiPath}/codedebug/{{sessionId}}", async (Guid sessionId,
                IMediator mediator, MapsterMapper.IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new DeleteCodeDebugCommand(sessionId), cancellationToken);

            var response = result.Adapt<DeleteCodeDebugRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("DeleteCodeDebug")
            .WithApiVersionSet(builder.NewApiVersionSet("CodeDebug").Build())
            .Produces<DeleteCodeDebugRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Delete CodeDebug")
            .WithDescription("Delete CodeDebug")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class DeleteCodeDebugCommandValidator : AbstractValidator<DeleteCodeDebugCommand>
{
    public DeleteCodeDebugCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
    }
}

internal class DeleteCodeDebugHandler : IRequestHandler<DeleteCodeDebugCommand, DeleteCodeDebugCommandResponse>
{
    private readonly CodeDebugDbContext _dbContext;

    public DeleteCodeDebugHandler(CodeDebugDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeleteCodeDebugCommandResponse> Handle(DeleteCodeDebugCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var codedebug = await _dbContext.CodeDebugs.FindAsync(new object[] { SessionId.Of(request.SessionId) }, cancellationToken);

        if (codedebug == null)
        {
            throw new CodeDebugNotFoundException(request.SessionId);
        }

        codedebug.Delete();
        _dbContext.CodeDebugs.Remove(codedebug);

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new DeleteCodeDebugCommandResponse(codedebug.Id.Value);
    }
}

