using AI.Common.Core;
using ImageCaption.ValueObjects;

namespace ImageCaption.Events;

public record ImageCaptionSessionCompletedDomainEvent(ImageCaptionId Id): IDomainEvent;