namespace AI.Common.Core;

/// <summary>
/// The standard input for any paginated request.
/// It includes which page you want, how many items per page, and optional filtering/sorting strings.
/// </summary>
public interface IPageRequest
{
    int PageNumber { get; init; }
    int PageSize { get; init; }
    string? Filters { get; init; }
    string? SortOrder { get; init; }
}