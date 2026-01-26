using AI.Common.Core;
using AiOrchestration.ValueObjects;

namespace AiOrchestration.Events;

public record AiModelCreatedDomainEvent(ModelId Id, string Name, Provider Provider) : IDomainEvent;
public record AiModelUpdatedDomainEvent(ModelId Id, string Name) : IDomainEvent;
public record AiModelStatusChangedDomainEvent(ModelId Id, bool IsActive) : IDomainEvent;
