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

namespace Payment.Features.RecordUsageCharge.V1;


public record RecordUsageChargeCommand(Guid SubscriptionId, string TokenUsed, string Description, decimal Cost, string Currency, string Module) : ICommand<RecordUsageChargeCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record RecordUsageChargeCommandResponse(Guid Id);

public record RecordUsageChargeRequest(Guid SubscriptionId, string TokenUsed, string Description, decimal Cost, string Currency, string Module);

public record RecordUsageChargeRequestResponse(Guid Id);

public class RecordUsageChargeEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/subscription/charge/record", async (RecordUsageChargeRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<RecordUsageChargeCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<RecordUsageChargeRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("RecordUsageCharge")
            .WithApiVersionSet(builder.NewApiVersionSet("Payment").Build())
            .Produces<RecordUsageChargeRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Record Usage Charge")
            .WithDescription("Record Usage Charge")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class RecordUsageChargeCommandValidator : AbstractValidator<RecordUsageChargeCommand>
{
    public RecordUsageChargeCommandValidator()
    {
        RuleFor(x => x.SubscriptionId).NotEmpty();
        RuleFor(x => x.Cost).GreaterThanOrEqualTo(0);
    }
}

internal class RecordUsageChargeHandler : IRequestHandler<RecordUsageChargeCommand, RecordUsageChargeCommandResponse>
{
    private readonly PaymentDbContext _dbContext;

    public RecordUsageChargeHandler(PaymentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RecordUsageChargeCommandResponse> Handle(RecordUsageChargeCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var subscription = await _dbContext.Subscriptions.FindAsync(new object[] { SubscriptionId.Of(request.SubscriptionId) }, cancellationToken);

        if (subscription == null)
        {
            throw new SubscriptionNotFoundException(request.SubscriptionId);
        }

        var charge = UsageCharge.Create(
            UsageChargeId.Of(NewId.NextGuid()),
            subscription.Id,
            subscription.UserId,
            request.TokenUsed,
            request.Description,
            Money.Of(request.Cost, request.Currency),
            request.Module);

        subscription.AddCharge(charge);

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new RecordUsageChargeCommandResponse(charge.Id.Value);
    }
}

