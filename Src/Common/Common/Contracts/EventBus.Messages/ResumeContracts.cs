using AI.Common.Core;

namespace AI.Contracts.EventBus.Messages;

public record ResumeAnalysisFailed(Guid Id) : IIntegrationEvent;
public record ResumeAnalyzed(Guid Id) : IIntegrationEvent;
public record ResumeParsed(Guid Id) : IIntegrationEvent;
public record ResumeUploaded(Guid Id) : IIntegrationEvent;