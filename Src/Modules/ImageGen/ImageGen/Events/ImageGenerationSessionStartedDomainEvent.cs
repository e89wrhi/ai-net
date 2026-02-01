using AI.Common.Core;
using AiOrchestration.ValueObjects;
using ImageGen.ValueObjects;

namespace ImageGen.Events;

public record ImageGenerationSessionStartedDomainEvent(ImageGenId Id, UserId UserId, ModelId AiModelId) : IDomainEvent;
