using AI.Common.Core;
using Resume.ValueObjects;

namespace Resume.Events;

public record ResumeParsedDomainEvent(ResumeId ResumeId, int TextLength) : IDomainEvent;
