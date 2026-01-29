using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using ImageCaption.Data;
using ImageCaption.ValueObjects;
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
using ImageCaption.Exceptions;
using ImageCaption.Models;

namespace ImageCaption.Features.GenerateCaption.V1;

public record GenerateCaptionCommand(Guid ImageId, string CaptionText, double Confidence, string Language) : ICommand<GenerateCaptionCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record GenerateCaptionCommandResponse(Guid Id);

public record GenerateCaptionRequest(Guid ImageId, string CaptionText, double Confidence, string Language);

public record GenerateCaptionRequestResponse(Guid Id);


public class GenerateCaptionEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/image/generate-caption", async (GenerateCaptionRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<GenerateCaptionCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<GenerateCaptionRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GenerateCaption")
            .WithApiVersionSet(builder.NewApiVersionSet("Image").Build())
            .Produces<GenerateCaptionRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Generate Caption")
            .WithDescription("Generate Caption")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class GenerateCaptionCommandValidator : AbstractValidator<GenerateCaptionCommand>
{
    public GenerateCaptionCommandValidator()
    {
        RuleFor(x => x.ImageId).NotEmpty();
        RuleFor(x => x.CaptionText).NotEmpty();
    }
}

internal class GenerateCaptionHandler : IRequestHandler<GenerateCaptionCommand, GenerateCaptionCommandResponse>
{
    private readonly ImageDbContext _dbContext;

    public GenerateCaptionHandler(ImageDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GenerateCaptionCommandResponse> Handle(GenerateCaptionCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var image = await _dbContext.Images.FindAsync(new object[] { ImageId.Of(request.ImageId) }, cancellationToken);

        if (image == null)
        {
            throw new ImageNotFoundException(request.ImageId);
        }

        var caption = CaptionModel.Create(
            CaptionId.Of(NewId.NextGuid()),
            image.Id,
            request.CaptionText,
            request.Confidence,
            request.Language);

        image.AddCaption(caption);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new GenerateCaptionCommandResponse(caption.Id.Value);
    }
}

