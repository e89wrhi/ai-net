using AI.Common.Core;
using ImageCaption.Enums;
using ImageCaption.ValueObjects;

namespace ImageCaption.Events;

public record ImageCaptionSessionFailedDomainEvent(ImageCaptionId Id, ImageCaptionFailureReason Reason): IDomainEvent;
