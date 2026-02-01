using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using TextToSpeech.Data;
using TextToSpeech.Exceptions;
using TextToSpeech.Models;
using TextToSpeech.ValueObjects;
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

namespace TextToSpeech.Features.SendTextToSpeech.V1;

public record SendTextToSpeechCommand(Guid SessionId, string Content, string Sender, int TokenUsed) : ICommand<SendTextToSpeechCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record SendTextToSpeechCommandResponse(Guid Id);

public record SendTextToSpeechRequest(Guid SessionId, string Content, string Sender, int TokenUsed);

public record SendTextToSpeechRequestResponse(Guid Id);

public class SendTextToSpeechEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/texttospeech/send-message", async (SendTextToSpeechRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<SendTextToSpeechCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<SendTextToSpeechRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("SendTextToSpeech")
            .WithApiVersionSet(builder.NewApiVersionSet("TextToSpeech").Build())
            .Produces<SendTextToSpeechRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Send TextToSpeech")
            .WithDescription("Send TextToSpeech")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class SendTextToSpeechCommandValidator : AbstractValidator<SendTextToSpeechCommand>
{
    public SendTextToSpeechCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
        RuleFor(x => x.Content).NotEmpty();
    }
}

internal class SendTextToSpeechHandler : IRequestHandler<SendTextToSpeechCommand, SendTextToSpeechCommandResponse>
{
    private readonly TextToSpeechDbContext _dbContext;

    public SendTextToSpeechHandler(TextToSpeechDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SendTextToSpeechCommandResponse> Handle(SendTextToSpeechCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var texttospeech = await _dbContext.TextToSpeechs.FindAsync(new object[] { SessionId.Of(request.SessionId) }, cancellationToken);

        if (texttospeech == null)
        {
            throw new TextToSpeechNotFoundException(request.SessionId);
        }

        var message = TextToSpeechModel.Create(
            TextToSpeechId.Of(NewId.NextGuid()),
            texttospeech.Id,
            ValueObjects.TextToSpeechConfiguration.Of(request.Sender),
            ValueObjects.SynthesizedSpeech.Of(request.Content),
            TokenUsed.Of(request.TokenUsed));

        texttospeech.AddTextToSpeech(message);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SendTextToSpeechCommandResponse(message.Id.Value);
    }
}

