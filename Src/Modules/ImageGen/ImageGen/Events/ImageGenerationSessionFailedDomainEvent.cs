using AI.Common.Core;
using ImageGen.Enums;
using ImageGen.ValueObjects;

namespace ImageGen.Events;

public record ImageGenerationSessionFailedDomainEvent(ImageGenId Id, ImageGenerationFailureReason Reason): IDomainEvent;
