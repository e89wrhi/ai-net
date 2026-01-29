using AI.Common.Core;
using Resume.Enums;
using Resume.ValueObjects;
using System.Linq;


namespace Resume.Models;

public record ResumeModel : Aggregate<ResumeId>
{
    public CandidateName CandidateName { get; private set; } = default!;
    public FileReference FileReference { get; private set; } = default!;
    public ParsedText ParsedText { get; private set; } = default!;
    public SourceFile SourceFile { get; private set; } = default!;
    public ResumeStatus ResumeStatus { get; private set; } = default!;
    public string UserId { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string Summary { get; private set; } = default!;


    private readonly List<ExpirenceModel> _expirences = new();
    public IReadOnlyCollection<ExpirenceModel> Expirences => _expirences.AsReadOnly();


    private readonly List<SkillModel> _skills = new();
    public IReadOnlyCollection<SkillModel> Skills => _skills.AsReadOnly();


    private readonly List<SuggestionModel> _suggestions = new();
    public IReadOnlyCollection<SuggestionModel> Suggestions => _suggestions.AsReadOnly();

    public DateTime? AnalyzedAt { get; private set; } = default!;

    private ResumeModel() { }

    public static ResumeModel Create(ResumeId id, string userId, CandidateName name, FileReference file)
    {
        var resume = new ResumeModel
        {
            Id = id,
            UserId = userId,
            CandidateName = name,
            FileReference = file,
            ResumeStatus = ResumeStatus.Uploaded,
            CreatedAt = DateTime.UtcNow
        };

        resume.AddDomainEvent(new Resume.Events.ResumeUploadedDomainEvent(id, userId, name.Value, file.FileName));
        return resume;
    }


    public void AddSkill(SkillModel skill)
    {
        _skills.Add(skill);
    }

    public void AddSuggestion(SuggestionModel suggestion)
    {
        _suggestions.Add(suggestion);
    }

    public void CompleteAnalysis(string summary, ParsedText parsedText)
    {
        Summary = summary;
        ParsedText = parsedText;
        ResumeStatus = ResumeStatus.Analyzed;
        AnalyzedAt = DateTime.UtcNow;
        LastModified = DateTime.UtcNow;

        AddDomainEvent(new Resume.Events.ResumeParsedDomainEvent(Id, parsedText.Value.Length));
        AddDomainEvent(new Resume.Events.ResumeAnalyzedDomainEvent(Id, summary, parsedText.Value, _skills.Select(x => x.Name).ToList(), _suggestions.Select(x => x.Description).ToList()));

    }
}
