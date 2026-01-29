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
using Meeting.Enums;
using Meeting.Exceptions;

namespace Meeting.Features.SummarizeMeetingAudio.V1;


public record SummarizeMeetingAudioCommand(Guid MeetingId, string TranscriptionText, string Language, double ConfidenceScore, string Summary) : ICommand<SummarizeMeetingAudioCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record SummarizeMeetingAudioCommandResponse(Guid Id);

public record SummarizeMeetingAudioRequest(Guid MeetingId, string TranscriptionText, string Language, double ConfidenceScore, string Summary);

public record SummarizeMeetingAudioRequestResponse(Guid Id);

public class SummarizeMeetingAudioEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/meeting/summarize", async (SummarizeMeetingAudioRequest request,
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
        RuleFor(x => x.MeetingId).NotEmpty();
        RuleFor(x => x.TranscriptionText).NotEmpty();
        RuleFor(x => x.Summary).NotEmpty();
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

        var meeting = await _dbContext.Meetings.FindAsync(new object[] { MeetingId.Of(request.MeetingId) }, cancellationToken);

        if (meeting == null)
        {
            throw new MeetingNotFoundException(request.MeetingId);
        }

        meeting.CompleteTranscription(
            request.TranscriptionText,
            Enum.Parse<TranscriptLanguage>(request.Language),
            request.ConfidenceScore,
            request.Summary);

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new SummarizeMeetingAudioCommandResponse(meeting.Id.Value);
    }
}


