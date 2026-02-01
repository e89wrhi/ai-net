using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using SpeechToText.Data;
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

namespace SpeechToText.Features.StartSpeechToText.V1;

public record StartSpeechToTextCommand(Guid UserId, string Title, string AiModelId) : ICommand<StartSpeechToTextCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record StartSpeechToTextCommandResponse(Guid Id);

public record StartSpeechToTextRequest(Guid UserId, string Title, string AiModelId);

public record StartSpeechToTextRequestResponse(Guid Id);

public class StartSpeechToTextEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/speechtotext", async (StartSpeechToTextRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<StartSpeechToTextCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<StartSpeechToTextRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("StartSpeechToText")
            .WithApiVersionSet(builder.NewApiVersionSet("SpeechToText").Build())
            .Produces<StartSpeechToTextRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Start SpeechToText")
            .WithDescription("Start SpeechToText")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class StartSpeechToTextCommandValidator : AbstractValidator<StartSpeechToTextCommand>
{
    public StartSpeechToTextCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.AiModelId).NotEmpty();
    }
}

internal class StartSpeechToTextHandler : IRequestHandler<StartSpeechToTextCommand, StartSpeechToTextCommandResponse>
{
    private readonly SpeechToTextDbContext _dbContext;

    public StartSpeechToTextHandler(SpeechToTextDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StartSpeechToTextCommandResponse> Handle(StartSpeechToTextCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var speechtotext = SpeechToTextModel.Create(
            SessionId.Of(NewId.NextGuid()),
            UserId.Of(request.UserId),
            request.Title,
            request.AiModelId);

        await _dbContext.SpeechToTexts.AddAsync(speechtotext, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new StartSpeechToTextCommandResponse(speechtotext.Id.Value);
    }
}

