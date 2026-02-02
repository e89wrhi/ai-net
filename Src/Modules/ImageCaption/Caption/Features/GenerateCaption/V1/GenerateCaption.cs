using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using ImageCaption.Data;
using ImageCaption.ValueObjects;
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
using ImageCaption.Exceptions;
using ImageCaption.Models;

namespace ImageCaption.Features.GenerateCaption.V1;

public class GenerateCaptionEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/image/generate-caption", async (GenerateCaptionRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<GenerateCaptionCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<GenerateCaptionRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GenerateCaption")
            .WithApiVersionSet(builder.NewApiVersionSet("Image").Build())
            .Produces<GenerateCaptionRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Generate Caption")
            .WithDescription("Generate Caption")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

