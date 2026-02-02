using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using ImageEdit.Data;
using ImageEdit.Models;
using ImageEdit.ValueObjects;
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

namespace ImageEdit.Features.StartImageEdit.V1;

public class StartImageEditEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/imageedit", async (StartImageEditRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<StartImageEditCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<StartImageEditRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StartImageEdit")
            .WithApiVersionSet(builder.NewApiVersionSet("ImageEdit").Build())
            .Produces<StartImageEditRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Start ImageEdit")
            .WithDescription("Start ImageEdit")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
