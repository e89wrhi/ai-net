using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using User.Data;
using User.ValueObjects;
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
using User.Exceptions;

namespace User.Features.ResetUsageCounters.V1;


public record ResetUsageCounterCommand(Guid UserId) : ICommand<ResetUsageCounterCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record ResetUsageCounterCommandResponse(Guid Id);

public record ResetUsageCounterRequest(Guid UserId);

public record ResetUsageCounterRequestResponse(Guid Id);

public class ResetUsageCounterEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/users/reset-usage", async (ResetUsageCounterRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<ResetUsageCounterCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<ResetUsageCounterRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("ResetUsageCounter")
            .WithApiVersionSet(builder.NewApiVersionSet("User").Build())
            .Produces<ResetUsageCounterRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Reset Usage Counter")
            .WithDescription("Reset Usage Counter")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class ResetUsageCounterCommandValidator : AbstractValidator<ResetUsageCounterCommand>
{
    public ResetUsageCounterCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}

internal class ResetUsageCounterHandler : IRequestHandler<ResetUsageCounterCommand, ResetUsageCounterCommandResponse>
{
    private readonly UserDbContext _dbContext;

    public ResetUsageCounterHandler(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ResetUsageCounterCommandResponse> Handle(ResetUsageCounterCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var user = await _dbContext.Users.FindAsync(new object[] { UserId.Of(request.UserId) }, cancellationToken);

        if (user == null)
        {
            throw new UserNotFoundException(request.UserId);
        }

        user.ResetUsages();

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new ResetUsageCounterCommandResponse(user.Id.Value);
    }
}

