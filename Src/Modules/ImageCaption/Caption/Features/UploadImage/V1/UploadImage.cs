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
using ImageCaption.Models;

namespace ImageCaption.Features.UploadImage.V1;

public record UploadImageCommand(string UserId, string ImageUrl, string FileName, int Width, int Height, long SizeInBytes, string Format) : ICommand<UploadImageCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record UploadImageCommandResponse(Guid Id);

public record UploadImageRequest(string UserId, string ImageUrl, string FileName, int Width, int Height, long SizeInBytes, string Format);

public record UploadImageRequestResponse(Guid Id);

public class UploadImageEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/image/upload", async (UploadImageRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<UploadImageCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<UploadImageRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("UploadImage")
            .WithApiVersionSet(builder.NewApiVersionSet("Image").Build())
            .Produces<UploadImageRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Upload Image")
            .WithDescription("Upload Image")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class UploadImageCommandValidator : AbstractValidator<UploadImageCommand>
{
    public UploadImageCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ImageUrl).NotEmpty();
    }
}

internal class UploadImageHandler : IRequestHandler<UploadImageCommand, UploadImageCommandResponse>
{
    private readonly ImageDbContext _dbContext;

    public UploadImageHandler(ImageDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UploadImageCommandResponse> Handle(UploadImageCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var image = ImageModel.Create(
            ImageId.Of(NewId.NextGuid()),
            request.UserId,
            FileReference.Of(request.ImageUrl, request.FileName),
            request.Width,
            request.Height,
            request.SizeInBytes,
            request.Format);

        await _dbContext.Images.AddAsync(image, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new UploadImageCommandResponse(image.Id.Value);
    }
}

