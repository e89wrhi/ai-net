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

namespace Resume.Features.AnalyzeResume.V1;


public record AnalyzeResumeCommand(Guid ResumeId, string Summary, string ParsedText) : ICommand<AnalyzeResumeCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record AnalyzeResumeCommandResponse(Guid Id);

public record AnalyzeResumeRequest(Guid ResumeId, string Summary, string ParsedText);

public record AnalyzeResumeRequestResponse(Guid Id);

public class AnalyzeResumeEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/resume/analyze", async (AnalyzeResumeRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<AnalyzeResumeCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<AnalyzeResumeRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("AnalyzeResume")
            .WithApiVersionSet(builder.NewApiVersionSet("Resume").Build())
            .Produces<AnalyzeResumeRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Analyze Resume")
            .WithDescription("Analyze Resume")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class AnalyzeResumeCommandValidator : AbstractValidator<AnalyzeResumeCommand>
{
    public AnalyzeResumeCommandValidator()
    {
        RuleFor(x => x.ResumeId).NotEmpty();
        RuleFor(x => x.Summary).NotEmpty();
        RuleFor(x => x.ParsedText).NotEmpty();
    }
}

internal class AnalyzeResumeHandler : IRequestHandler<AnalyzeResumeCommand, AnalyzeResumeCommandResponse>
{
    private readonly ResumeDbContext _dbContext;

    public AnalyzeResumeHandler(ResumeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AnalyzeResumeCommandResponse> Handle(AnalyzeResumeCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var resume = await _dbContext.Resumes.FindAsync(new object[] { ResumeId.Of(request.ResumeId) }, cancellationToken);

        if (resume == null)
        {
            throw new ResumeNotFoundException(request.ResumeId);
        }

        resume.CompleteAnalysis(request.Summary, ParsedText.Of(request.ParsedText));

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new AnalyzeResumeCommandResponse(resume.Id.Value);
    }
}

