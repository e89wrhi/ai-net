using AI.Common.Core;
using ImageEdit.ValueObjects;

namespace ImageEdit.Events;

public record ImageEditSessionCompletedDomainEvent(ImageEditId Id) : IDomainEvent;
