using AI.Common.Core;
using MassTransit;
using User.Enums;

namespace User.Features.TrackActivity.V1;

public record TrackActivityCommand(Guid UserId, TrackedModule Module, string Action, Guid ResourceId, string IpAddress, string UserAgent) : ICommand<TrackActivityCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record TrackActivityCommandResponse(Guid Id);
