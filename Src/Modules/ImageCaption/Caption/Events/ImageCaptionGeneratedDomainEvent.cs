using AI.Common.Core;
using ImageCaption.ValueObjects;

namespace ImageCaption.Events;

public record ImageCaptionGeneratedDomainEvent(ImageCaptionId Id, ImageCaptionResultId ResultId, string Caption): IDomainEvent;
