namespace AI.Common.EventStoreDB;

public interface IProjection
{
    void When(object @event);
}