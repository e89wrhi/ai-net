using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using Payment.Data;
using Payment.ValueObjects;
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
using Payment.Exceptions;
using Payment.Models;

namespace Payment.Features.GenerateInvoice.V1;


public class GenerateInvoiceEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/subscription/invoice/generate", async (GenerateInvoiceRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<GenerateInvoiceCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<GenerateInvoiceRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GenerateInvoice")
            .WithApiVersionSet(builder.NewApiVersionSet("Payment").Build())
            .Produces<GenerateInvoiceRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Generate Invoice")
            .WithDescription("Generate Invoice")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

