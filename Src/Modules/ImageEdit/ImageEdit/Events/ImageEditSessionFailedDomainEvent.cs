using AI.Common.Core;
using ImageEdit.Enums;
using ImageEdit.ValueObjects;

namespace ImageEdit.Events;

public record ImageEditSessionFailedDomainEvent(ImageEditId Id, ImageEditFailureReason Reason): IDomainEvent;

