using AI.Common.Core;

namespace AI.Common.Contracts.EventBus.Messages;

public record VoteCreated(Guid Id) : IIntegrationEvent;
public record VoteDeleted(Guid Id) : IIntegrationEvent;