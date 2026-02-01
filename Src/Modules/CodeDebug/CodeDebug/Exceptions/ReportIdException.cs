using AI.Common.BaseExceptions;

namespace CodeDebug.Exceptions;

public class ReportIdException : DomainException
{
    public ReportIdException(Guid id)
        : base($"report_id: '{id}' is invalid.")
    {
    }
}