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

namespace Meeting.Features.UploadMeetingAudio.V1;


public record UploadMeetingAudioCommand() : ICommand<UploadMeetingAudioCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record UploadMeetingAudioCommandResponse(Guid Id);

public record UploadMeetingAudioRequest();

public record UploadMeetingAudioRequestResponse(Guid Id);

public class UploadMeetingAudioEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/meeting", async (UploadMeetingAudioRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<UploadMeetingAudioCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<UploadMeetingAudioRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("UploadMeetingAudio")
            .WithApiVersionSet(builder.NewApiVersionSet("Meeting").Build())
            .Produces<UploadMeetingAudioRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Upload Meeting Audio")
            .WithDescription("Upload Meeting Audio")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class UploadMeetingAudioCommandValidator : AbstractValidator<UploadMeetingAudioCommand>
{
    public UploadMeetingAudioCommandValidator()
    {
    }
}

internal class UploadMeetingAudioHandler : IRequestHandler<UploadMeetingAudioCommand, UploadMeetingAudioCommandResponse>
{
    private readonly MeetingDbContext _dbContext;

    public UploadMeetingAudioHandler(MeetingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UploadMeetingAudioCommandResponse> Handle(UploadMeetingAudioCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        await _dbContext.SaveChangesAsync(cancellationToken);
        return new UploadMeetingAudioCommandResponse(newMeeting.Id);
    }
}
