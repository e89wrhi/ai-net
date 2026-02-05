using Ardalis.GuardClauses;
using MassTransit;
using MediatR;
using Payment.Data;
using Payment.Exceptions;
using Payment.Models;
using Payment.ValueObjects;

namespace Payment.Features.GenerateInvoice.V1;

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

        var invoiceNumber = $"INV-{subscription.Id.Value.ToString().Substring(0, 8)}-{DateTime.UtcNow:yyyyMMdd}";
        var invoice = Payment.Models.Invoice.Create(
            InvoiceId.Of(NewId.NextGuid()),
            invoiceNumber,
            Money.Of(request.Amount, request.Currency),
            CurrencyCode.Of(request.Currency));

        subscription.AddInvoice(invoice);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new GenerateInvoiceCommandResponse(invoice.Id.Value);
    }
}


