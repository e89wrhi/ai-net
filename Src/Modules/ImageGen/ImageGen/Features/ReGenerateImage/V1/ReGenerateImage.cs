using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using ImageGen.Data;
using ImageGen.Exceptions;
using ImageGen.Models;
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

namespace ImageGen.Features.SendImageGen.V1;

public record SendImageGenCommand(Guid SessionId, string Content, string Sender, int TokenUsed) : ICommand<SendImageGenCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record SendImageGenCommandResponse(Guid Id);

public record SendImageGenRequest(Guid SessionId, string Content, string Sender, int TokenUsed);

public record SendImageGenRequestResponse(Guid Id);

public class SendImageGenEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/imagegen/send-message", async (SendImageGenRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<SendImageGenCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<SendImageGenRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("SendImageGen")
            .WithApiVersionSet(builder.NewApiVersionSet("ImageGen").Build())
            .Produces<SendImageGenRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Send ImageGen")
            .WithDescription("Send ImageGen")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class SendImageGenCommandValidator : AbstractValidator<SendImageGenCommand>
{
    public SendImageGenCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
        RuleFor(x => x.Content).NotEmpty();
    }
}

internal class SendImageGenHandler : IRequestHandler<SendImageGenCommand, SendImageGenCommandResponse>
{
    private readonly ImageGenDbContext _dbContext;

    public SendImageGenHandler(ImageGenDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SendImageGenCommandResponse> Handle(SendImageGenCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var imagegen = await _dbContext.ImageGens.FindAsync(new object[] { SessionId.Of(request.SessionId) }, cancellationToken);

        if (imagegen == null)
        {
            throw new ImageGenNotFoundException(request.SessionId);
        }

        var message = ImageGenModel.Create(
            ImageGenId.Of(NewId.NextGuid()),
            imagegen.Id,
            ValueObjects.ImageGenerationPrompt.Of(request.Sender),
            ValueObjects.GeneratedImage.Of(request.Content),
            TokenUsed.Of(request.TokenUsed));

        imagegen.AddImageGen(message);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SendImageGenCommandResponse(message.Id.Value);
    }
}

