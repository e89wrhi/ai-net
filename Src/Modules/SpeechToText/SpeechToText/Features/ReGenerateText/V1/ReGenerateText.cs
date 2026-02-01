using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using SpeechToText.Data;
using SpeechToText.Exceptions;
using SpeechToText.Models;
using SpeechToText.ValueObjects;
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

namespace SpeechToText.Features.SendSpeechToText.V1;

public record SendSpeechToTextCommand(Guid SessionId, string Content, string Sender, int TokenUsed) : ICommand<SendSpeechToTextCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record SendSpeechToTextCommandResponse(Guid Id);

public record SendSpeechToTextRequest(Guid SessionId, string Content, string Sender, int TokenUsed);

public record SendSpeechToTextRequestResponse(Guid Id);

public class SendSpeechToTextEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/speechtotext/send-message", async (SendSpeechToTextRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<SendSpeechToTextCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<SendSpeechToTextRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("SendSpeechToText")
            .WithApiVersionSet(builder.NewApiVersionSet("SpeechToText").Build())
            .Produces<SendSpeechToTextRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Send SpeechToText")
            .WithDescription("Send SpeechToText")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class SendSpeechToTextCommandValidator : AbstractValidator<SendSpeechToTextCommand>
{
    public SendSpeechToTextCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
        RuleFor(x => x.Content).NotEmpty();
    }
}

internal class SendSpeechToTextHandler : IRequestHandler<SendSpeechToTextCommand, SendSpeechToTextCommandResponse>
{
    private readonly SpeechToTextDbContext _dbContext;

    public SendSpeechToTextHandler(SpeechToTextDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SendSpeechToTextCommandResponse> Handle(SendSpeechToTextCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var speechtotext = await _dbContext.SpeechToTexts.FindAsync(new object[] { SessionId.Of(request.SessionId) }, cancellationToken);

        if (speechtotext == null)
        {
            throw new SpeechToTextNotFoundException(request.SessionId);
        }

        var message = SpeechToTextModel.Create(
            SpeechToTextId.Of(NewId.NextGuid()),
            speechtotext.Id,
            ValueObjects.SpeechToTextConfiguration.Of(request.Sender),
            ValueObjects.Transcript.Of(request.Content),
            ValueObjects.AudioSource.Of(request.TokenUsed));

        speechtotext.AddSpeechToText(message);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SendSpeechToTextCommandResponse(message.Id.Value);
    }
}

