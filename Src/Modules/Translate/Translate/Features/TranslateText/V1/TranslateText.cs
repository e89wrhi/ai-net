using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using Translate.Data;
using Translate.Exceptions;
using Translate.ValueObjects;
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

namespace Translate.Features.DeleteTranslate.V1;

public record DeleteTranslateCommand(Guid SessionId) : ICommand<DeleteTranslateCommandResponse>;

public record DeleteTranslateCommandResponse(Guid Id);

public record DeleteTranslateRequest(Guid SessionId);

public record DeleteTranslateRequestResponse(Guid Id);

public class DeleteTranslateEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete($"{EndpointConfig.BaseApiPath}/translate/{{sessionId}}", async (Guid sessionId,
                IMediator mediator, MapsterMapper.IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new DeleteTranslateCommand(sessionId), cancellationToken);

            var response = result.Adapt<DeleteTranslateRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("DeleteTranslate")
            .WithApiVersionSet(builder.NewApiVersionSet("Translate").Build())
            .Produces<DeleteTranslateRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Delete Translate")
            .WithDescription("Delete Translate")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class DeleteTranslateCommandValidator : AbstractValidator<DeleteTranslateCommand>
{
    public DeleteTranslateCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
    }
}

internal class DeleteTranslateHandler : IRequestHandler<DeleteTranslateCommand, DeleteTranslateCommandResponse>
{
    private readonly TranslateDbContext _dbContext;

    public DeleteTranslateHandler(TranslateDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeleteTranslateCommandResponse> Handle(DeleteTranslateCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var translate = await _dbContext.Translates.FindAsync(new object[] { SessionId.Of(request.SessionId) }, cancellationToken);

        if (translate == null)
        {
            throw new TranslateNotFoundException(request.SessionId);
        }

        translate.Delete();
        _dbContext.Translates.Remove(translate);

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new DeleteTranslateCommandResponse(translate.Id.Value);
    }
}

