namespace SimpleMD.Services;

/// <summary>
/// Configuration options for the markdown file used as AI context.
/// Bind from appsettings under the "SimpleMD" section.
/// </summary>
public sealed class MarkdownFileOptions
{
    public const string SectionName = "SimpleMD";

    /// <summary>
    /// Relative (to <see cref="AppContext.BaseDirectory"/>) or absolute path to
    /// the markdown file that is injected as context into every AI prompt.
    /// Defaults to "context.md".
    /// </summary>
    public string FilePath { get; init; } = "context.md";
}
