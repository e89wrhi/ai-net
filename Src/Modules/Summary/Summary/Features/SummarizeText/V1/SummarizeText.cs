using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using Summary.Data;
using Summary.Exceptions;
using Summary.ValueObjects;
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

namespace Summary.Features.DeleteSummary.V1;

public record DeleteSummaryCommand(Guid SessionId) : ICommand<DeleteSummaryCommandResponse>;

public record DeleteSummaryCommandResponse(Guid Id);

public record DeleteSummaryRequest(Guid SessionId);

public record DeleteSummaryRequestResponse(Guid Id);

public class DeleteSummaryEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete($"{EndpointConfig.BaseApiPath}/summary/{{sessionId}}", async (Guid sessionId,
                IMediator mediator, MapsterMapper.IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new DeleteSummaryCommand(sessionId), cancellationToken);

            var response = result.Adapt<DeleteSummaryRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("DeleteSummary")
            .WithApiVersionSet(builder.NewApiVersionSet("Summary").Build())
            .Produces<DeleteSummaryRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Delete Summary")
            .WithDescription("Delete Summary")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class DeleteSummaryCommandValidator : AbstractValidator<DeleteSummaryCommand>
{
    public DeleteSummaryCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
    }
}

internal class DeleteSummaryHandler : IRequestHandler<DeleteSummaryCommand, DeleteSummaryCommandResponse>
{
    private readonly SummaryDbContext _dbContext;

    public DeleteSummaryHandler(SummaryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeleteSummaryCommandResponse> Handle(DeleteSummaryCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var summary = await _dbContext.Summarys.FindAsync(new object[] { SessionId.Of(request.SessionId) }, cancellationToken);

        if (summary == null)
        {
            throw new SummaryNotFoundException(request.SessionId);
        }

        summary.Delete();
        _dbContext.Summarys.Remove(summary);

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new DeleteSummaryCommandResponse(summary.Id.Value);
    }
}

