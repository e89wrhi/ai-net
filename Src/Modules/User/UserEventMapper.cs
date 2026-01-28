using AI.Common.Contracts.EventBus.Messages;
using AI.Common.Core;
using AI.Contracts.EventBus.Messages;
using User.Events;
using User.Features.CreateUser.V1;
using User.Features.ResetUsageCounters.V1;
using User.Features.TrackActivity.V1;

using User.Features.UpdateUser.V1;


namespace User;


// ref: https://www.ledjonbehluli.com/posts/domain_to_integration_event/
public sealed class UserEventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            UserActivityTrackedDomainEvent e => new AI.Contracts.EventBus.Messages.UserActivityAdded(e.ActivityId.Value, e.UserId.Value, e.Module.ToString(), e.Action),
            UserProfileUpdatedDomainEvent e => new AI.Contracts.EventBus.Messages.UserProfileUpdated(e.Id.Value),
            _ => null
        };
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            UserActivityTrackedDomainEvent e => new TrackActivityMongo(e.ActivityId.Value, e.UserId.Value, e.Module.ToString(), e.Action, e.ResourceId, e.TimeStamp),
            UsageCountersResetDomainEvent e => new ResetUsageCountersMongo(e.UserId.Value),
            UserCreatedDomainEvent e => new CreateUserMongo(e.UserId.Value, e.Username, e.Email),
            UserProfileUpdatedDomainEvent e => new UpdateUserMongo(e.Id.Value, e.FullName),
            _ => null



        };
    }
}

