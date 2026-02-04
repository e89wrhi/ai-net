using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Google.Protobuf.WellKnownTypes;
using Payment;
using Payment.Features.GetInvoices.V1;

namespace Payment.GrpcServer.Services;

public class PaymentGrpcService : Payment.PaymentGrpcService.PaymentGrpcServiceBase
{
    private readonly IMediator _mediator;

    public PaymentGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<RecordUsageChargeResponse> RecordUsageCharge(RecordUsageChargeRequest request, ServerCallContext context)
    {
        var cmd = new Payment.Features.RecordUsageCharge.V1.RecordUsageChargeCommand(
            Guid.Parse(request.SubscriptionId),
            request.TokenUsed,
            request.Description,
            Convert.ToDecimal(request.Cost),
            request.Currency,
            request.Module);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new RecordUsageChargeResponse { Id = result.Id.ToString() };
    }

    public override async Task<GetInvoicesResponse> GetInvoices(GetInvoicesRequest request, ServerCallContext context)
    {
        var query = new GetInvoices(Guid.Parse(request.SubscriptionId), request.UserId);
        var result = await _mediator.Send(query, context.CancellationToken);

        var response = new GetInvoicesResponse();
        foreach (var inv in result.InvoiceDtos)
        {
            var protoInv = new Invoice
            {
                Id = inv.Id.ToString(),
                Amount = Convert.ToDouble(inv.Amount),
                Currency = inv.Currency ?? string.Empty,
                Status = inv.Status ?? string.Empty,
                InvoiceNumber = inv.InvoiceNumber ?? string.Empty
            };

            if (inv.IssuedAt != default)
            {
                var utc = DateTime.SpecifyKind(inv.IssuedAt.ToUniversalTime(), DateTimeKind.Utc);
                protoInv.IssuedAt = Timestamp.FromDateTime(utc);
            }

            response.Invoices.Add(protoInv);
        }

        return response;
    }
}
