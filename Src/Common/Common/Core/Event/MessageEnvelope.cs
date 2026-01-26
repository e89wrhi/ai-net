using Google.Protobuf;

namespace AI.Common.Core;

/// <summary>
/// A wrapper for our messages (Events or Commands) that allows us to attach extra metadata (Headers).
/// This is very useful for cross-cutting concerns like Correlation IDs and User Identity.
/// </summary>
public class MessageEnvelope
{
    public MessageEnvelope(object? message, IDictionary<string, object?>? headers = null)
    {
        Message = message;
        Headers = headers ?? new Dictionary<string, object?>();
    }

    public object? Message { get; init; }
    public IDictionary<string, object?> Headers { get; init; }
}

public class MessageEnvelope<TMessage> : MessageEnvelope
    where TMessage : class, IMessage
{
    public MessageEnvelope(TMessage message, IDictionary<string, object?> header) : base(message, header)
    {
        Message = message;
    }

    public new TMessage? Message { get; }
}