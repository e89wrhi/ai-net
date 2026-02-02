using MassTransit;
using MediatR;
using User.Data;
using User.ValueObjects;

namespace User.Features.TrackActivity.V1;


internal class TrackActivityHandler : IRequestHandler<TrackActivityCommand, TrackActivityCommandResponse>
{
    private readonly UserDbContext _dbContext;

    public TrackActivityHandler(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TrackActivityCommandResponse> Handle(TrackActivityCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var user = await _dbContext.Users.FindAsync(new object[] { UserId.Of(request.UserId) }, cancellationToken);

        if (user == null)
        {
            throw new UserNotFoundException(request.UserId);
        }

        var activity = User.UserActivity.Create(
            UserAnalyticsId.Of(NewId.NextGuid()),
            user.Id,
            request.Module,
            request.Action,
            request.ResourceId,
            request.IpAddress,
            request.UserAgent);

        user.TrackActivity(activity);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new TrackActivityCommandResponse(activity.Id.Value);
    }
}

