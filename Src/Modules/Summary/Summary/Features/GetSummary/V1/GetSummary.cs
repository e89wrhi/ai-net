using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using Summary.Data;
using Summary.Models;
using Summary.ValueObjects;
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

namespace Summary.Features.StartSummary.V1;

public record StartSummaryCommand(Guid UserId, string Title, string AiModelId) : ICommand<StartSummaryCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record StartSummaryCommandResponse(Guid Id);

public record StartSummaryRequest(Guid UserId, string Title, string AiModelId);

public record StartSummaryRequestResponse(Guid Id);

public class StartSummaryEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/summary", async (StartSummaryRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<StartSummaryCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<StartSummaryRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StartSummary")
            .WithApiVersionSet(builder.NewApiVersionSet("Summary").Build())
            .Produces<StartSummaryRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Start Summary")
            .WithDescription("Start Summary")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class StartSummaryCommandValidator : AbstractValidator<StartSummaryCommand>
{
    public StartSummaryCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.AiModelId).NotEmpty();
    }
}

internal class StartSummaryHandler : IRequestHandler<StartSummaryCommand, StartSummaryCommandResponse>
{
    private readonly SummaryDbContext _dbContext;

    public StartSummaryHandler(SummaryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StartSummaryCommandResponse> Handle(StartSummaryCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var summary = SummaryModel.Create(
            SessionId.Of(NewId.NextGuid()),
            UserId.Of(request.UserId),
            request.Title,
            request.AiModelId);

        await _dbContext.Summarys.AddAsync(summary, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new StartSummaryCommandResponse(summary.Id.Value);
    }
}

