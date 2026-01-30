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


public record GenerateInvoiceCommand(Guid SubscriptionId, decimal Amount, string Currency, string LineItems) : ICommand<GenerateInvoiceCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record GenerateInvoiceCommandResponse(Guid Id);

public record GenerateInvoiceRequest(Guid SubscriptionId, decimal Amount, string Currency, string LineItems);

public record GenerateInvoiceRequestResponse(Guid Id);

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

public class GenerateInvoiceCommandValidator : AbstractValidator<GenerateInvoiceCommand>
{
    public GenerateInvoiceCommandValidator()
    {
        RuleFor(x => x.SubscriptionId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}

internal class GenerateInvoiceHandler : IRequestHandler<GenerateInvoiceCommand, GenerateInvoiceCommandResponse>
{
    private readonly PaymentDbContext _dbContext;

    public GenerateInvoiceHandler(PaymentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GenerateInvoiceCommandResponse> Handle(GenerateInvoiceCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var subscription = await _dbContext.Subscriptions.FindAsync(new object[] { SubscriptionId.Of(request.SubscriptionId) }, cancellationToken);

        if (subscription == null)
        {
            throw new SubscriptionNotFoundException(request.SubscriptionId);
        }

        var invoice = InvoiceModel.Create(
            InvoiceId.Of(NewId.NextGuid()),
            subscription.Id,
            subscription.UserId,
            BillingPeriod.Of(DateTime.UtcNow.AddMonths(1)),
            Money.Of(request.Amount),
            request.LineItems,
            $"INV-{subscription.Id.Value.ToString().Substring(0, 8)}-{DateTime.UtcNow:yyyyMMdd}");

        subscription.AddInvoice(invoice);

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new GenerateInvoiceCommandResponse(invoice.Id.Value);
    }
}

