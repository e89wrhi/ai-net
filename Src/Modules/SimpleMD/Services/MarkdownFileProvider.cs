using Microsoft.Extensions.Options;

namespace SimpleMD.Services;

/// <summary>
/// Reads a single markdown file from disk. The path is resolved relative to
/// <see cref="AppContext.BaseDirectory"/> and is configurable via
/// <see cref="MarkdownFileOptions"/>.
/// </summary>
public sealed class MarkdownFileProvider : IMarkdownFileProvider
{
    private readonly string _filePath;

    public MarkdownFileProvider(IOptions<MarkdownFileOptions> options)
    {
        var opts = options.Value;
        _filePath = Path.IsPathRooted(opts.FilePath)
            ? opts.FilePath
            : Path.Combine(AppContext.BaseDirectory, opts.FilePath);
    }

    public async Task<string> GetMarkdownAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
            throw new FileNotFoundException("Configured markdown context file not found.", _filePath);

        return await File.ReadAllTextAsync(_filePath, cancellationToken);
    }
}
