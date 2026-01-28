using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using Resume.Data;
using Resume.ValueObjects;
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

namespace Resume.Features.ReAnalyzeResume.V1;


public record ReAnalyzeResumeCommand() : ICommand<ReAnalyzeResumeCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record ReAnalyzeResumeCommandResponse(Guid Id);

public record ReAnalyzeResumeRequest();

public record ReAnalyzeResumeRequestResponse(Guid Id);

public class ReAnalyzeResumeEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/resume", async (ReAnalyzeResumeRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<ReAnalyzeResumeCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<ReAnalyzeResumeRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("ReAnalyzeResume")
            .WithApiVersionSet(builder.NewApiVersionSet("Resume").Build())
            .Produces<ReAnalyzeResumeRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("ReAnalyze Resume")
            .WithDescription("ReAnalyze Resume")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class ReAnalyzeResumeCommandValidator : AbstractValidator<ReAnalyzeResumeCommand>
{
    public ReAnalyzeResumeCommandValidator()
    {
    }
}

internal class ReAnalyzeResumeHandler : IRequestHandler<ReAnalyzeResumeCommand, ReAnalyzeResumeCommandResponse>
{
    private readonly ResumeDbContext _dbContext;

    public ReAnalyzeResumeHandler(ResumeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ReAnalyzeResumeCommandResponse> Handle(ReAnalyzeResumeCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        await _dbContext.SaveChangesAsync(cancellationToken);
        return new ReAnalyzeResumeCommandResponse(newResume.Id);
    }
}
