using AI.Common.Core;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using User.Data;
using User.Models;
using User.ValueObjects;
using Ardalis.GuardClauses;

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
        Ardalis.GuardClauses.Guard.Against.Null(request, nameof(request));

        var userId = UserId.Of(request.UserId);

        // Find an active session for this user or create a new one
        var session = await _dbContext.Sessions
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Status == Enums.UserActivityStatus.Active, cancellationToken);

        if (session == null)
        {
            session = UserActivitySession.Create(UserActivityId.Of(NewId.NextGuid()), userId);
            _dbContext.Sessions.Add(session);
        }

        // Record the action
        var action = UserAction.Create(request.Action, $"Action on {request.Module} resource {request.ResourceId}");
        session.RecordAction(action);

        // Update user analytics
        var analytics = await _dbContext.UserAnalytics
            .FirstOrDefaultAsync(x => x.User == userId, cancellationToken);
        
        if (analytics == null)
        {
            analytics = UserAnalytics.Create(UserAnalyticsId.Of(NewId.NextGuid()), userId);
            _dbContext.UserAnalytics.Add(analytics);
        }
        
        analytics.Increment(DateTime.UtcNow);

        // Update module analytics
        // Generate a deterministic Guid from the module enum value
        var moduleGuid = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, (byte)request.Module + 1);
        var moduleId = ModuleId.Of(moduleGuid);
        var moduleAnalytics = await _dbContext.ModuleAnalytics
            .FirstOrDefaultAsync(x => x.Module == moduleId, cancellationToken);
        
        if (moduleAnalytics == null)
        {
            moduleAnalytics = ModuleAnalytics.Create(ModuleAnalyticsId.Of(NewId.NextGuid()), moduleId);
            _dbContext.ModuleAnalytics.Add(moduleAnalytics);
        }
        
        moduleAnalytics.Increment(DateTime.UtcNow);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new TrackActivityCommandResponse(action.Id.Value);
    }
}

