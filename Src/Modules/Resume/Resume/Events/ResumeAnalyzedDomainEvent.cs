using AI.Common.Core;
using Resume.ValueObjects;
using System.Collections.Generic;

namespace Resume.Events;

public record ResumeAnalyzedDomainEvent(ResumeId ResumeId, string Summary, string ParsedText, List<string> Skills, List<string> Suggestions) : IDomainEvent;
