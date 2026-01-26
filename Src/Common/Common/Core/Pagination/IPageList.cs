namespace AI.Common.Core;

/// <summary>
/// A standardized container for paginated data.
/// It tells the client not just the items on the current page, but also metadata like TotalCount and TotalPages.
/// </summary>
public interface IPageList<T>
    where T : class
{
    int CurrentPageSize { get; }
    int CurrentStartIndex { get; }
    int CurrentEndIndex { get; }
    int TotalPages { get; }
    bool HasPrevious { get; }
    bool HasNext { get; }
    IReadOnlyList<T> Items { get; init; }
    int TotalCount { get; init; }
    int PageNumber { get; init; }
    int PageSize { get; init; }
}