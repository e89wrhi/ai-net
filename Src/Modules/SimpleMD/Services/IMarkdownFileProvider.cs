namespace SimpleMD.Services;

/// <summary>
/// Provides the raw markdown content loaded from a well-known file on disk.
/// Abstracting file I/O here makes handlers testable and the path configurable.
/// </summary>
public interface IMarkdownFileProvider
{
    /// <summary>Reads and returns the full markdown content.</summary>
    Task<string> GetMarkdownAsync(CancellationToken cancellationToken = default);
}
