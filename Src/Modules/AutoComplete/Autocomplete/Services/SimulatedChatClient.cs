using System.Runtime.CompilerServices;
using Microsoft.Extensions.AI;

namespace AutoComplete.Services;

public class SimulatedChatClient : IChatClient
{
    private readonly ChatClientMetadata _metadata;

    public SimulatedChatClient()
    {
        _metadata = new ChatClientMetadata("SimulatedChatClient", new Uri("http://localhost"), "simulated-model");
    }

    public ChatClientMetadata Metadata => _metadata;

    public object? GetService(Type serviceType, object? key = null) =>
        key is not null ? null :
        serviceType == typeof(ChatClientMetadata) ? _metadata :
        serviceType?.IsInstanceOfType(this) is true ? this :
        null;

    public async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> chatMessages, 
        ChatOptions? options = null, 
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(10, cancellationToken);
        var lastMessage = chatMessages.LastOrDefault();
        var prompt = lastMessage?.Text ?? "No prompt";

        // Real usecase simulation: provide autocomplete suggestions
        var suggestions = $"Here are some suggestions based on '{prompt}':\n1. {prompt} extended\n2. {prompt} refined\n3. {prompt} alternative";

        return new ChatResponse(new[] { new ChatMessage(ChatRole.Assistant, suggestions) });
    }

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> chatMessages, 
        ChatOptions? options = null, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var lastMessage = chatMessages.LastOrDefault();
        var prompt = lastMessage?.Text ?? "No prompt";

        yield return new (ChatRole.System, "Simulated ");
        await Task.Delay(10, cancellationToken);
        yield return new (ChatRole.System, "Suggestion ");
        await Task.Delay(10, cancellationToken);
        yield return new (ChatRole.System, $"for: {prompt}");
    }

    public void Dispose()
    {
        // No-op
    }
}


