using AI.Common.Core;
using Resume.Enums;
using Resume.ValueObjects;

namespace Resume.Models;

public record ResumeModel : Aggregate<ResumeId>
{
    public CandidateName CandidateName { get; private set; } = default!;
    public FileReference FileReference { get; private set; } = default!;
    public ParsedText ParsedText { get; private set; } = default!;
    public SourceFile SourceFile { get; private set; } = default!;
    public ResumeStatus ResumeStatus { get; private set; } = default!;


    private readonly List<ExpirenceModel> _expirences = new();
    public IReadOnlyCollection<ExpirenceModel> Expirences => _expirences.AsReadOnly();


    private readonly List<SkillModel> _skills = new();
    public IReadOnlyCollection<SkillModel> Skills => _skills.AsReadOnly();


    private readonly List<SuggestionModel> _suggestions = new();
    public IReadOnlyCollection<SuggestionModel> Suggestions => _suggestions.AsReadOnly();

    public DateTime? AnalyzedAt { get; private set; } = default!;
}
