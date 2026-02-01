using AI.Common.Core;
using AiOrchestration.ValueObjects;
using ImageCaption.ValueObjects;

namespace ImageCaption.Events;

public record ImageCaptionSessionStartedDomainEvent(ImageCaptionId Id, UserId UserId, ModelId AiModel) : IDomainEvent;
