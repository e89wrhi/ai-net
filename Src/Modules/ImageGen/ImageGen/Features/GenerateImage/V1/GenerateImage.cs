using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using ImageGen.Data;
using ImageGen.Exceptions;
using ImageGen.ValueObjects;
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

namespace ImageGen.Features.DeleteImageGen.V1;

public record DeleteImageGenCommand(Guid SessionId) : ICommand<DeleteImageGenCommandResponse>;

public record DeleteImageGenCommandResponse(Guid Id);

public record DeleteImageGenRequest(Guid SessionId);

public record DeleteImageGenRequestResponse(Guid Id);

public class DeleteImageGenEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete($"{EndpointConfig.BaseApiPath}/imagegen/{{sessionId}}", async (Guid sessionId,
                IMediator mediator, MapsterMapper.IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new DeleteImageGenCommand(sessionId), cancellationToken);

            var response = result.Adapt<DeleteImageGenRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("DeleteImageGen")
            .WithApiVersionSet(builder.NewApiVersionSet("ImageGen").Build())
            .Produces<DeleteImageGenRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Delete ImageGen")
            .WithDescription("Delete ImageGen")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class DeleteImageGenCommandValidator : AbstractValidator<DeleteImageGenCommand>
{
    public DeleteImageGenCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
    }
}

internal class DeleteImageGenHandler : IRequestHandler<DeleteImageGenCommand, DeleteImageGenCommandResponse>
{
    private readonly ImageGenDbContext _dbContext;

    public DeleteImageGenHandler(ImageGenDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeleteImageGenCommandResponse> Handle(DeleteImageGenCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var imagegen = await _dbContext.ImageGens.FindAsync(new object[] { SessionId.Of(request.SessionId) }, cancellationToken);

        if (imagegen == null)
        {
            throw new ImageGenNotFoundException(request.SessionId);
        }

        imagegen.Delete();
        _dbContext.ImageGens.Remove(imagegen);

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new DeleteImageGenCommandResponse(imagegen.Id.Value);
    }
}

