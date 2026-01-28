using AI.Common.Core;
using ImageCaption.ValueObjects;

namespace ImageCaption.Events;

public record ImageUploadedDomainEvent(ImageId ImageId, string UserId, string FilePath, int Width, int Height, long Size, string Format) : IDomainEvent;
public record ImageAnalyzedDomainEvent(ImageId ImageId, int ObjectCount) : IDomainEvent;
public record CaptionGeneratedDomainEvent(ImageId ImageId, string Caption) : IDomainEvent;
