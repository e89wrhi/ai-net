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


public record UploadResumeCommand(string UserId, string CandidateName, string ResumeUrl, string FileName) : ICommand<UploadResumeCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record UploadResumeCommandResponse(Guid Id);

public record UploadResumeRequest(string UserId, string CandidateName, string ResumeUrl, string FileName);

public record UploadResumeRequestResponse(Guid Id);

public class UploadResumeEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/resume/upload", async (UploadResumeRequest request,
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
            .WithSummary("Upload Resume")
            .WithDescription("Upload Resume")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class UploadResumeCommandValidator : AbstractValidator<UploadResumeCommand>
{
    public UploadResumeCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.CandidateName).NotEmpty();
        RuleFor(x => x.ResumeUrl).NotEmpty();
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

        var resume = ResumeModel.Create(
            ResumeId.Of(NewId.NextGuid()),
            request.UserId,
            CandidateName.Of(request.CandidateName),
            FileReference.Of(request.ResumeUrl, request.FileName));

        await _dbContext.Resumes.AddAsync(resume, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new UploadResumeCommandResponse(resume.Id.Value);
    }
}


