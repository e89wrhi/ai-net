using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using ImageEdit.Data;
using ImageEdit.Exceptions;
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

namespace ImageEdit.Features.DeleteImageEdit.V1;

public record DeleteImageEditCommand(Guid SessionId) : ICommand<DeleteImageEditCommandResponse>;

public record DeleteImageEditCommandResponse(Guid Id);

public record DeleteImageEditRequest(Guid SessionId);

public record DeleteImageEditRequestResponse(Guid Id);

public class DeleteImageEditEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete($"{EndpointConfig.BaseApiPath}/imageedit/{{sessionId}}", async (Guid sessionId,
                IMediator mediator, MapsterMapper.IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new DeleteImageEditCommand(sessionId), cancellationToken);

            var response = result.Adapt<DeleteImageEditRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("DeleteImageEdit")
            .WithApiVersionSet(builder.NewApiVersionSet("ImageEdit").Build())
            .Produces<DeleteImageEditRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Delete ImageEdit")
            .WithDescription("Delete ImageEdit")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class DeleteImageEditCommandValidator : AbstractValidator<DeleteImageEditCommand>
{
    public DeleteImageEditCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
    }
}

internal class DeleteImageEditHandler : IRequestHandler<DeleteImageEditCommand, DeleteImageEditCommandResponse>
{
    private readonly ImageEditDbContext _dbContext;

    public DeleteImageEditHandler(ImageEditDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeleteImageEditCommandResponse> Handle(DeleteImageEditCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var imageedit = await _dbContext.ImageEdits.FindAsync(new object[] { SessionId.Of(request.SessionId) }, cancellationToken);

        if (imageedit == null)
        {
            throw new ImageEditNotFoundException(request.SessionId);
        }

        imageedit.Delete();
        _dbContext.ImageEdits.Remove(imageedit);

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new DeleteImageEditCommandResponse(imageedit.Id.Value);
    }
}

