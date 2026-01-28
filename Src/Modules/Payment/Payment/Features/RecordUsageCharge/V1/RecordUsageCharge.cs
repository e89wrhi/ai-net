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


public record RecordUsageChargeCommand() : ICommand<RecordUsageChargeCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record RecordUsageChargeCommandResponse(Guid Id);

public record RecordUsageChargeRequest();

public record RecordUsageChargeRequestResponse(Guid Id);

public class RecordUsageChargeEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/payment", async (RecordUsageChargeRequest request,
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

        await _dbContext.SaveChangesAsync(cancellationToken);
        return new RecordUsageChargeCommandResponse(newPayment.Id);
    }
}
