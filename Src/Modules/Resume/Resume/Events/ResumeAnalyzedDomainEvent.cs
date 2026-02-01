using AI.Common.Core;
using Resume.ValueObjects;
using System.Collections.Generic;

namespace Resume.Events;

public record ResumeAnalyzedDomainEvent(ResumeId ResumeId, ResultId ResultId, string Summary) : IDomainEvent;
