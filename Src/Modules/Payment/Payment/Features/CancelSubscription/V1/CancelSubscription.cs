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

namespace Payment.Features.CancelSubscription.V1;


public record CancelSubscriptionCommand() : ICommand<CancelSubscriptionCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record CancelSubscriptionCommandResponse(Guid Id);

public record CancelSubscriptionRequest();

public record CancelSubscriptionRequestResponse(Guid Id);

public class CancelSubscriptionEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/payment", async (CancelSubscriptionRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<CancelSubscriptionCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<CancelSubscriptionRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("CancelSubscription")
            .WithApiVersionSet(builder.NewApiVersionSet("Payment").Build())
            .Produces<CancelSubscriptionRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Cancel Subscription")
            .WithDescription("Cancel Subscription")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class CancelSubscriptionCommandValidator : AbstractValidator<CancelSubscriptionCommand>
{
    public CancelSubscriptionCommandValidator()
    {
    }
}

internal class CancelSubscriptionHandler : IRequestHandler<CancelSubscriptionCommand, CancelSubscriptionCommandResponse>
{
    private readonly PaymentDbContext _dbContext;

    public CancelSubscriptionHandler(PaymentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CancelSubscriptionCommandResponse> Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        await _dbContext.SaveChangesAsync(cancellationToken);
        return new CancelSubscriptionCommandResponse(newMeeting.Id);
    }
}
