using CodeDebug.Enums;
using CodeDebug.Exceptions;

namespace CodeDebug.ValueObjects;

public record CodeDebugConfiguration
{
    public DebugDepth Depth { get; }
    public bool IncludeSuggestions { get; }

    public CodeDebugConfiguration(DebugDepth depth, bool includeSuggestions)
    {
        Depth = depth;
        IncludeSuggestions = includeSuggestions;
    }
}
