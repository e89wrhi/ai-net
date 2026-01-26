namespace AI.Common.MassTransit;

/// <summary>
/// Configuration settings for connecting to RabbitMQ.
/// Includes host information, authentication, and the default exchange name used for messaging.
/// </summary>
public class RabbitMqOptions
{
    public string HostName { get; set; }
    public string ExchangeName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public ushort? Port { get; set; }
}