using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace User.Features.TrackActivity.V1;


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
            .ProducesProblem(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Track Activity")
            .WithDescription("Track Activity")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

