using CodeDebug.Exceptions;

namespace CodeDebug.ValueObjects;

public record CodeDebugReportId
{
    public Guid Value { get; }

    private CodeDebugReportId(Guid value)
    {
        Value = value;
    }

    public static CodeDebugReportId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ReportIdException(value);
        }

        return new CodeDebugReportId(value);
    }

    public static implicit operator Guid(CodeDebugReportId id)
    {
        return id.Value;
    }
}