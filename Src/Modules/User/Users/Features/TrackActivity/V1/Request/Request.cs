using User.Enums;

namespace User.Features.TrackActivity.V1;


public record TrackActivityRequest(Guid UserId, TrackedModule Module, string Action, Guid ResourceId);

public record TrackActivityRequestResponse(Guid Id);
