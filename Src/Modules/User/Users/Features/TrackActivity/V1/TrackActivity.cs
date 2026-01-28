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

namespace User.Features.TrackActivity.V1;


public record TrackActivityCommand() : ICommand<TrackActivityCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record TrackActivityCommandResponse(Guid Id);

public record TrackActivityRequest();

public record TrackActivityRequestResponse(Guid Id);

public class TrackActivityEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/users", async (TrackActivityRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<TrackActivityCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<TrackActivityRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("TrackActivity")
            .WithApiVersionSet(builder.NewApiVersionSet("User").Build())
            .Produces<TrackActivityRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Track Activity")
            .WithDescription("Track Activity")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class TrackActivityCommandValidator : AbstractValidator<TrackActivityCommand>
{
    public TrackActivityCommandValidator()
    {
    }
}

internal class TrackActivityHandler : IRequestHandler<TrackActivityCommand, TrackActivityCommandResponse>
{
    private readonly UserDbContext _dbContext;

    public TrackActivityHandler(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TrackActivityCommandResponse> Handle(TrackActivityCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        await _dbContext.SaveChangesAsync(cancellationToken);
        return new TrackActivityCommandResponse(newUser.Id);
    }
}
