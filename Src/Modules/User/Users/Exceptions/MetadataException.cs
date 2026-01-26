using AI.Common.BaseExceptions;

namespace User.Exceptions;

public class MetadataException : DomainException
{
    public MetadataException(string metadata)
        : base($"metadata: '{metadata}' is invalid.")
    {
    }
}
