using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using CodeGen.Data;
using CodeGen.Exceptions;
using CodeGen.ValueObjects;
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

namespace CodeGen.Features.DeleteCodeGen.V1;

public record DeleteCodeGenCommand(Guid SessionId) : ICommand<DeleteCodeGenCommandResponse>;

public record DeleteCodeGenCommandResponse(Guid Id);

public record DeleteCodeGenRequest(Guid SessionId);

public record DeleteCodeGenRequestResponse(Guid Id);

public class DeleteCodeGenEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete($"{EndpointConfig.BaseApiPath}/codegen/{{sessionId}}", async (Guid sessionId,
                IMediator mediator, MapsterMapper.IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new DeleteCodeGenCommand(sessionId), cancellationToken);

            var response = result.Adapt<DeleteCodeGenRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("DeleteCodeGen")
            .WithApiVersionSet(builder.NewApiVersionSet("CodeGen").Build())
            .Produces<DeleteCodeGenRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Delete CodeGen")
            .WithDescription("Delete CodeGen")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class DeleteCodeGenCommandValidator : AbstractValidator<DeleteCodeGenCommand>
{
    public DeleteCodeGenCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
    }
}

internal class DeleteCodeGenHandler : IRequestHandler<DeleteCodeGenCommand, DeleteCodeGenCommandResponse>
{
    private readonly CodeGenDbContext _dbContext;

    public DeleteCodeGenHandler(CodeGenDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeleteCodeGenCommandResponse> Handle(DeleteCodeGenCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var codegen = await _dbContext.CodeGens.FindAsync(new object[] { SessionId.Of(request.SessionId) }, cancellationToken);

        if (codegen == null)
        {
            throw new CodeGenNotFoundException(request.SessionId);
        }

        codegen.Delete();
        _dbContext.CodeGens.Remove(codegen);

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new DeleteCodeGenCommandResponse(codegen.Id.Value);
    }
}

