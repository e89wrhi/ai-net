using AI.Common.Core;
using ImageEdit.ValueObjects;

namespace ImageEdit.Events;

public record ImageEditedDomainEvent(ImageEditId Id, ImageEditResultId ResultId) : IDomainEvent;
