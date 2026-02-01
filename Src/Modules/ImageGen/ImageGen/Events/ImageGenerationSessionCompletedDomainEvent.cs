using AI.Common.Core;
using ImageGen.ValueObjects;

namespace ImageGen.Events;

public record ImageGenerationSessionCompletedDomainEvent(ImageGenId Id): IDomainEvent;

