using AI.Common.BaseExceptions;

namespace ImageGen.Exceptions;

public class GeneratedImageException : DomainException
{
    public GeneratedImageException(string img)
        : base($"generated_image: '{img}' is invalid.")
    {
    }
}