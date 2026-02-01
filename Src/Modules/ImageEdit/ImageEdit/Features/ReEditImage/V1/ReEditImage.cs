using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using ImageEdit.Data;
using ImageEdit.Exceptions;
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

namespace ImageEdit.Features.SendImageEdit.V1;

public record SendImageEditCommand(Guid SessionId, string Content, string Sender, int TokenUsed) : ICommand<SendImageEditCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record SendImageEditCommandResponse(Guid Id);

public record SendImageEditRequest(Guid SessionId, string Content, string Sender, int TokenUsed);

public record SendImageEditRequestResponse(Guid Id);

public class SendImageEditEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/imageedit/send-message", async (SendImageEditRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<SendImageEditCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<SendImageEditRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("SendImageEdit")
            .WithApiVersionSet(builder.NewApiVersionSet("ImageEdit").Build())
            .Produces<SendImageEditRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Send ImageEdit")
            .WithDescription("Send ImageEdit")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class SendImageEditCommandValidator : AbstractValidator<SendImageEditCommand>
{
    public SendImageEditCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
        RuleFor(x => x.Content).NotEmpty();
    }
}

internal class SendImageEditHandler : IRequestHandler<SendImageEditCommand, SendImageEditCommandResponse>
{
    private readonly ImageEditDbContext _dbContext;

    public SendImageEditHandler(ImageEditDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SendImageEditCommandResponse> Handle(SendImageEditCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var imageedit = await _dbContext.ImageEdits.FindAsync(new object[] { SessionId.Of(request.SessionId) }, cancellationToken);

        if (imageedit == null)
        {
            throw new ImageEditNotFoundException(request.SessionId);
        }

        var message = ImageEditModel.Create(
            ImageEditId.Of(NewId.NextGuid()),
            imageedit.Id,
            ValueObjects.ImageEditPrompt.Of(request.Sender),
            ValueObjects.EditedImage.Of(request.Content),
            TokenUsed.Of(request.TokenUsed));

        imageedit.AddImageEdit(message);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SendImageEditCommandResponse(message.Id.Value);
    }
}

