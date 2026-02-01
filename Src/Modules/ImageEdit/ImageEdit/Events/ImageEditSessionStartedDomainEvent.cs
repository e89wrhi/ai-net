using AI.Common.Core;
using AiOrchestration.ValueObjects;
using ImageEdit.ValueObjects;

namespace ImageEdit.Events;

public record ImageEditSessionStartedDomainEvent(ImageEditId Id, UserId UserId, ModelId AiModel) : IDomainEvent;
