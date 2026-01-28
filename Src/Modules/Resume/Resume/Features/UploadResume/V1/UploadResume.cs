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

namespace Resume.Features.UploadResume.V1;


public record UploadResumeCommand() : ICommand<UploadResumeCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record UploadResumeCommandResponse(Guid Id);

public record UploadResumeRequest();

public record UploadResumeRequestResponse(Guid Id);

public class UploadResumeEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/resume", async (UploadResumeRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<UploadResumeCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<UploadResumeRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("UploadResume")
            .WithApiVersionSet(builder.NewApiVersionSet("Resume").Build())
            .Produces<UploadResumeRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Update Resume")
            .WithDescription("Update Resume")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class UploadResumeCommandValidator : AbstractValidator<UploadResumeCommand>
{
    public UploadResumeCommandValidator()
    {
    }
}

internal class UploadResumeHandler : IRequestHandler<UploadResumeCommand, UploadResumeCommandResponse>
{
    private readonly ResumeDbContext _dbContext;

    public UploadResumeHandler(ResumeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UploadResumeCommandResponse> Handle(UploadResumeCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        await _dbContext.SaveChangesAsync(cancellationToken);
        return new UploadResumeCommandResponse(newResume.Id);
    }
}

