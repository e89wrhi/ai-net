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
using ImageCaption.Models;

namespace ImageCaption.Features.UploadImage.V1;

public class UploadImageEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/image/upload", async (UploadImageRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<UploadImageCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<UploadImageRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("UploadImage")
            .WithApiVersionSet(builder.NewApiVersionSet("Image").Build())
            .Produces<UploadImageRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Upload Image")
            .WithDescription("Upload Image")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
