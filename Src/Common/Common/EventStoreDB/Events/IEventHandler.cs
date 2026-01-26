using MediatR;
using AI.Common.Core;

namespace AI.Common.EventStoreDB;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : IEvent
{
}