using MediatR;

namespace AI.Common.EventStoreDB;

public interface IProjectionPublisher
{
    Task PublishAsync<T>(StreamEvent<T> streamEvent, CancellationToken cancellationToken = default)
        where T : INotification;

    Task PublishAsync(StreamEvent streamEvent, CancellationToken cancellationToken = default);
}