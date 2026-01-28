namespace Payment.Features.GenerateInvoice.V1;


using Ardalis.GuardClauses;
using Payment.Data;
using Payment.Models;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using AI.Common.Core;
using Payment.Exceptions;
using System;

public record GenerateInvoiceMongo(Guid SubscriptionId, Guid InvoiceId, decimal Amount, string Currency, string Status, string InvoiceNumber, DateTime IssuedAt) : InternalCommand;

public class GenerateInvoiceMongoHandler : ICommandHandler<GenerateInvoiceMongo>
{
    private readonly PaymentReadDbContext _readDbContext;

    public GenerateInvoiceMongoHandler(PaymentReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(GenerateInvoiceMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<SubscriptionReadModel>.Filter.Eq(x => x.Id, request.SubscriptionId);
        
        var invoice = new InvoiceReadModel
        {
            Id = request.InvoiceId,
            Amount = request.Amount,
            Currency = request.Currency,
            Status = request.Status,
            InvoiceNumber = request.InvoiceNumber,
            IssuedAt = request.IssuedAt
        };

        var update = Builders<SubscriptionReadModel>.Update.Push(x => x.Invoices, invoice);

        await _readDbContext.Subscription.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

