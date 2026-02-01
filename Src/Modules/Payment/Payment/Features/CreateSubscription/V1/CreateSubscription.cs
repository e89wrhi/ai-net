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
using Payment.Enums;
using Payment.Models;

namespace Payment.Features.CreateSubscription.V1;


public record CreateSubscriptionCommand(Guid UserId, Models.SubscriptionPlan Plan, int MaxRequestsPerDay, int MaxTokensPerMonth) : ICommand<CreateSubscriptionCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record CreateSubscriptionCommandResponse(Guid Id);

public record CreateSubscriptionRequest(Guid UserId, Models.SubscriptionPlan Plan, int MaxRequestsPerDay, int MaxTokensPerMonth);

public record CreateSubscriptionRequestResponse(Guid Id);

public class CreateSubscriptionEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/subscription/create", async (CreateSubscriptionRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<CreateSubscriptionCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<CreateSubscriptionRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("CreateSubscription")
            .WithApiVersionSet(builder.NewApiVersionSet("Payment").Build())
            .Produces<CreateSubscriptionRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Subscription")
            .WithDescription("Create Subscription")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class CreateSubscriptionCommandValidator : AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}

internal class CreateSubscriptionHandler : IRequestHandler<CreateSubscriptionCommand, CreateSubscriptionCommandResponse>
{
    private readonly PaymentDbContext _dbContext;

    public CreateSubscriptionHandler(PaymentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreateSubscriptionCommandResponse> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var subscription = SubscriptionModel.Create(
            SubscriptionId.Of(NewId.NextGuid()),
            UserId.Of(request.UserId),
            request.Plan.ToString(),
            request.MaxRequestsPerDay,
            request.MaxTokensPerMonth);

        await _dbContext.Subscriptions.AddAsync(subscription, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new CreateSubscriptionCommandResponse(subscription.Id.Value);
    }
}

