using AI.Common.Core;

namespace AI.Contracts.EventBus.Messages;

public record ResumeAnalysisFailed(Guid Id) : IIntegrationEvent;
public record ResumeAnalyzed(Guid Id) : IIntegrationEvent;
public record ResumeParsed(Guid Id) : IIntegrationEvent;
public record ResumeUploaded(Guid Id) : IIntegrationEvent;
public record ResumeScoreCalculated(Guid Id, double Score) : IIntegrationEvent;
public record CandidateShortlisted(Guid Id, Guid JobId) : IIntegrationEvent;
public record JobMatchFound(Guid Id, Guid JobId, double Probability) : IIntegrationEvent;
public record ResumeTemplateExported(Guid Id, string Format) : IIntegrationEvent;