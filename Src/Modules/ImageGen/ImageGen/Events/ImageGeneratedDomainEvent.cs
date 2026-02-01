using AI.Common.Core;
using ImageGen.ValueObjects;

namespace ImageGen.Events;

public record ImageGeneratedDomainEvent(ImageGenId Id, ImageGenResultId ResultId) : IDomainEvent;
