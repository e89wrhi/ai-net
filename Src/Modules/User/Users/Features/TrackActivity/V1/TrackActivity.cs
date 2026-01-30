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
using User.Enums;
using User.Exceptions;
using User.Models;

namespace User.Features.TrackActivity.V1;


public record TrackActivityCommand(Guid UserId, TrackedModule Module, string Action, Guid ResourceId, string IpAddress, string UserAgent) : ICommand<TrackActivityCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record TrackActivityCommandResponse(Guid Id);

public record TrackActivityRequest(Guid UserId, TrackedModule Module, string Action, Guid ResourceId);

public record TrackActivityRequestResponse(Guid Id);

public class TrackActivityEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/users/track-activity", async (TrackActivityRequest request,
                IMediator mediator, IMapper mapper,
                IHttpContextAccessor httpContextAccessor,
                CancellationToken cancellationToken) =>
        {
            var ipAddress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString() ?? "unknown";

            var command = new TrackActivityCommand(
                request.UserId,
                request.Module,
                request.Action,
                request.ResourceId,
                ipAddress,
                userAgent);

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
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Action).NotEmpty();
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

        var user = await _dbContext.Users.FindAsync(new object[] { UserId.Of(request.UserId) }, cancellationToken);

        if (user == null)
        {
            throw new UserNotFoundException(request.UserId);
        }

        var activity = User.UserActivity.Create(
            UserActivityId.Of(NewId.NextGuid()),
            user.Id,
            request.Module,
            request.Action,
            request.ResourceId,
            request.IpAddress,
            request.UserAgent);

        user.TrackActivity(activity);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new TrackActivityCommandResponse(activity.Id.Value);
    }
}

