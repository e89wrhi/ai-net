using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Google.Protobuf.WellKnownTypes;
using Payment.Features.GetInvoices.V1;
using Payment.Features.CreateSubscription.V1;
using Payment.Features.CancelSubscription.V1;
using Payment.Features.GetSubscription.V1;
using Payment.Features.GenerateInvoice.V1;
using Payment.Features.AnalyzeInvoice.V1;
using Payment.Features.ForecastSpending.V1;
using Payment.ValueObjects;
using Payment.GrpcServer.Protos;

using Protos = Payment.GrpcServer.Protos;

namespace Payment.GrpcServer.Services;

public class PaymentGrpcService : Protos.PaymentGrpcService.PaymentGrpcServiceBase
{
    private readonly IMediator _mediator;

    public PaymentGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<CreateSubscriptionResponse> CreateSubscription(Protos.CreateSubscriptionRequest request, ServerCallContext context)
    {
        var plan = new SubscriptionPlan(
            request.PlanName,
            Money.Of(Convert.ToDecimal(request.PlanPrice), request.PlanCurrency),
            (Payment.Enums.BillingCycle)(int)request.PlanCycle
        );

        var cmd = new CreateSubscriptionCommand(
            Guid.Parse(request.UserId),
            plan,
            request.MaxRequestsPerDay,
            request.MaxTokensPerMonth);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new CreateSubscriptionResponse { Id = result.Id.ToString() };
    }

    public override async Task<CancelSubscriptionResponse> CancelSubscription(Protos.CancelSubscriptionRequest request, ServerCallContext context)
    {
        var cmd = new CancelSubscriptionCommand(Guid.Parse(request.SubscriptionId));
        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new CancelSubscriptionResponse { Id = result.Id.ToString() };
    }

    public override async Task<GetSubscriptionResponse> GetSubscription(GetSubscriptionRequest request, ServerCallContext context)
    {
        var query = new GetSubscription(Guid.Parse(request.UserId));
        var result = await _mediator.Send(query, context.CancellationToken);

        if (result.SubscriptionDto == null)
        {
            return new GetSubscriptionResponse();
        }

        var sub = result.SubscriptionDto;
        var responseSub = new Subscription
        {
            Id = sub.Id.ToString(),
            UserId = sub.UserId.ToString(),
            Plan = sub.Plan,
            Status = sub.Status
        };

        if (sub.StartedAt.HasValue)
        {
            responseSub.StartedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(sub.StartedAt.Value.ToUniversalTime(), DateTimeKind.Utc));
        }

        if (sub.ExpiresAt.HasValue)
        {
            responseSub.ExpiresAt = Timestamp.FromDateTime(DateTime.SpecifyKind(sub.ExpiresAt.Value.ToUniversalTime(), DateTimeKind.Utc));
        }

        return new GetSubscriptionResponse { Subscription = responseSub };
    }

    public override async Task<GetInvoicesResponse> GetInvoices(GetInvoicesRequest request, ServerCallContext context)
    {
        var query = new GetInvoices(Guid.Parse(request.SubscriptionId), Guid.Parse(request.UserId));
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

    public override async Task<GenerateInvoiceResponse> GenerateInvoice(Protos.GenerateInvoiceRequest request, ServerCallContext context)
    {
        var cmd = new GenerateInvoiceCommand(
            Guid.Parse(request.SubscriptionId),
            Convert.ToDecimal(request.Amount),
            request.Currency,
            request.LineItems);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new GenerateInvoiceResponse { Id = result.Id.ToString() };
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

    public override async Task<AnalyzeInvoiceResponse> AnalyzeInvoice(AnalyzeInvoiceRequest request, ServerCallContext context)
    {
        var cmd = new AnalyzeInvoiceWithAICommand(Guid.Parse(request.InvoiceId));
        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new AnalyzeInvoiceResponse
        {
            Summary = result.Summary,
            Analysis = result.Analysis,
            HasAnomalies = result.HasAnomalies
        };
    }

    public override async Task<ForecastSpendingResponse> ForecastSpending(ForecastSpendingRequest request, ServerCallContext context)
    {
        var cmd = new ForecastSpendingWithAICommand(Guid.Parse(request.UserId));
        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new ForecastSpendingResponse
        {
            ForecastedAmount = Convert.ToDouble(result.ForecastedAmount),
            Currency = result.Currency,
            Insights = result.Insights
        };
    }
}

