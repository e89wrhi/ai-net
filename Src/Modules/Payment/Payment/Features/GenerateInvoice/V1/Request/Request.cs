namespace Payment.Features.GenerateInvoice.V1;

public record GenerateInvoiceRequest(Guid SubscriptionId, decimal Amount, string Currency, string LineItems);
public record GenerateInvoiceRequestResponse(Guid Id);
