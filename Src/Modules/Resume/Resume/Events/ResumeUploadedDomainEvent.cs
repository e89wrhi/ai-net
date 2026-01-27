using AI.Common.Core;
using Resume.ValueObjects;

namespace Resume.Events;

public record ResumeUploadedDomainEvent(ResumeId ResumeId, string UserId, string FileName) : IDomainEvent;
