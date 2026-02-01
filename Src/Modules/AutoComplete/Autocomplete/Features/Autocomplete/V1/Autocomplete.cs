using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using AutoComplete.Data;
using AutoComplete.Exceptions;
using AutoComplete.ValueObjects;
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

namespace AutoComplete.Features.DeleteAutoComplete.V1;

public record DeleteAutoCompleteCommand(Guid SessionId) : ICommand<DeleteAutoCompleteCommandResponse>;

public record DeleteAutoCompleteCommandResponse(Guid Id);

public record DeleteAutoCompleteRequest(Guid SessionId);

public record DeleteAutoCompleteRequestResponse(Guid Id);

public class DeleteAutoCompleteEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete($"{EndpointConfig.BaseApiPath}/autoComplete/{{sessionId}}", async (Guid sessionId,
                IMediator mediator, MapsterMapper.IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new DeleteAutoCompleteCommand(sessionId), cancellationToken);

            var response = result.Adapt<DeleteAutoCompleteRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("DeleteAutoComplete")
            .WithApiVersionSet(builder.NewApiVersionSet("AutoComplete").Build())
            .Produces<DeleteAutoCompleteRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Delete AutoComplete")
            .WithDescription("Delete AutoComplete")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class DeleteAutoCompleteCommandValidator : AbstractValidator<DeleteAutoCompleteCommand>
{
    public DeleteAutoCompleteCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
    }
}

internal class DeleteAutoCompleteHandler : IRequestHandler<DeleteAutoCompleteCommand, DeleteAutoCompleteCommandResponse>
{
    private readonly AutocompleteDbContext _dbContext;

    public DeleteAutoCompleteHandler(AutocompleteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeleteAutoCompleteCommandResponse> Handle(DeleteAutoCompleteCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var autoComplete = await _dbContext.AutoCompletes.FindAsync(new object[] { SessionId.Of(request.SessionId) }, cancellationToken);

        if (autoComplete == null)
        {
            throw new AutoCompleteNotFoundException(request.SessionId);
        }

        autocomplete.Delete();
        _dbContext.AutoCompletes.Remove(autocomplete);

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new DeleteAutoCompleteCommandResponse(autoComplete.Id.Value);
    }
}

