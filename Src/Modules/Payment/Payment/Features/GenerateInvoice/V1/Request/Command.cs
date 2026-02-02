using AI.Common.Core;
using MassTransit;

namespace Payment.Features.GenerateInvoice.V1;

public record GenerateInvoiceCommand(Guid SubscriptionId, decimal Amount, string Currency, string LineItems) : ICommand<GenerateInvoiceCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record GenerateInvoiceCommandResponse(Guid Id);
