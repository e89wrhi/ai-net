using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using Meeting.Data;
using Meeting.ValueObjects;
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

namespace Meeting.Features.SummarizeMeetingAudio.V1;


public record SummarizeMeetingAudioCommand() : ICommand<SummarizeMeetingAudioCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record SummarizeMeetingAudioCommandResponse(Guid Id);

public record SummarizeMeetingAudioRequest();

public record SummarizeMeetingAudioRequestResponse(Guid Id);

public class SummarizeMeetingAudioEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/meeting", async (SummarizeMeetingAudioRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<SummarizeMeetingAudioCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<SummarizeMeetingAudioRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("SummarizeMeetingAudio")
            .WithApiVersionSet(builder.NewApiVersionSet("Meeting").Build())
            .Produces<SummarizeMeetingAudioRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Summarize Meeting")
            .WithDescription("Summarize Meeting")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class SummarizeMeetingAudioCommandValidator : AbstractValidator<SummarizeMeetingAudioCommand>
{
    public SummarizeMeetingAudioCommandValidator()
    {
    }
}

internal class SummarizeMeetingAudioHandler : IRequestHandler<SummarizeMeetingAudioCommand, SummarizeMeetingAudioCommandResponse>
{
    private readonly MeetingDbContext _dbContext;

    public SummarizeMeetingAudioHandler(MeetingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SummarizeMeetingAudioCommandResponse> Handle(SummarizeMeetingAudioCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        await _dbContext.SaveChangesAsync(cancellationToken);
        return new SummarizeMeetingAudioCommandResponse(newAssistant.Id);
    }
}

