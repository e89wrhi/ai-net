using AI.Common.BaseExceptions;

namespace AiOrchestration.Exceptions;

public class TemperatureException : DomainException
{
    public TemperatureException(float temprature)
        : base($"temprature: '{temprature}' is invalid.")
    {
    }
}
