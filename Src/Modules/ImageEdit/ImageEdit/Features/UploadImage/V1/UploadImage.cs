using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using ImageEdit.Data;
using ImageEdit.Models;
using ImageEdit.ValueObjects;
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

namespace ImageEdit.Features.StartImageEdit.V1;

public record StartImageEditCommand(Guid UserId, string Title, string AiModelId) : ICommand<StartImageEditCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record StartImageEditCommandResponse(Guid Id);

public record StartImageEditRequest(Guid UserId, string Title, string AiModelId);

public record StartImageEditRequestResponse(Guid Id);

public class StartImageEditEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/imageedit", async (StartImageEditRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<StartImageEditCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<StartImageEditRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StartImageEdit")
            .WithApiVersionSet(builder.NewApiVersionSet("ImageEdit").Build())
            .Produces<StartImageEditRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Start ImageEdit")
            .WithDescription("Start ImageEdit")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class StartImageEditCommandValidator : AbstractValidator<StartImageEditCommand>
{
    public StartImageEditCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.AiModelId).NotEmpty();
    }
}

internal class StartImageEditHandler : IRequestHandler<StartImageEditCommand, StartImageEditCommandResponse>
{
    private readonly ImageEditDbContext _dbContext;

    public StartImageEditHandler(ImageEditDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StartImageEditCommandResponse> Handle(StartImageEditCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var imageedit = ImageEditModel.Create(
            SessionId.Of(NewId.NextGuid()),
            UserId.Of(request.UserId),
            request.Title,
            request.AiModelId);

        await _dbContext.ImageEdits.AddAsync(imageedit, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new StartImageEditCommandResponse(imageedit.Id.Value);
    }
}

