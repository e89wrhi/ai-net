using MassTransit;
using AI.Common.Core;
using AI.Common.PersistMessageProcessor;

namespace AI.Common.MassTransit;

// Handle inbox messages with masstransit pipeline
/// <summary>
/// A MassTransit filter that implements the "Idempotent Consumer" pattern.
/// It records every incoming message in the "Inbox" and ensures we don't process the same message twice,
/// even if the message broker delivers it multiple times.
/// </summary>
public class ConsumeFilter<T> : IFilter<ConsumeContext<T>>
    where T : class
{
    private readonly IPersistMessageProcessor _persistMessageProcessor;

    public ConsumeFilter(IPersistMessageProcessor persistMessageProcessor)
    {
        _persistMessageProcessor = persistMessageProcessor;
    }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        var id = await _persistMessageProcessor.AddReceivedMessageAsync(
            new MessageEnvelope(
                context.Message,
                context.Headers.ToDictionary(x => x.Key, x => x.Value))
        );

        var message = await _persistMessageProcessor.ExistMessageAsync(id);

        if (message is null)
        {
            await next.Send(context);
            await _persistMessageProcessor.ProcessInboxAsync(id);
        }
    }

    public void Probe(ProbeContext context)
    {
    }
}