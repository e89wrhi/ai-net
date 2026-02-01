using AI.Common.Contracts.EventBus.Messages;
using AI.Common.Core;
using Resume.Events;
using Resume.Features.AnalyzeResume.V1;
using Resume.Features.ReAnalyzeResume.V1;
using Resume.Features.UploadResume.V1;

namespace Resume;

public sealed class ResumeEventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            ResumeAnalysisSessionFailedDomainEvent e => new AI.Contracts.EventBus.Messages.ResumeUploaded(e.ResumeId.Value),
            ResumeAnalyzedDomainEvent e => new AI.Contracts.EventBus.Messages.ResumeAnalyzed(e.ResumeId.Value),
            ResumeAnalysisSessionCompletedDomainEvent e => new AI.Contracts.EventBus.Messages.ResumeParsed(e.ResumeId.Value),
            _ => null
        };
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            ResumeAnalysisSessionFailedDomainEvent e => new UploadResumeMongo(e.ResumeId.Value, e.UserId, e.CandidateName, e.FilePath, "Uploaded", DateTime.UtcNow),
            ResumeAnalyzedDomainEvent e => new AnalyzeResumeMongo(e.ResumeId.Value, e.Summary, e.ParsedText, e.Skills, e.Suggestions, "Analyzed", DateTime.UtcNow),
            _ => null
        };
    }
}