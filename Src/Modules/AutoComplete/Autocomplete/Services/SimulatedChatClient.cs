using System.Runtime.CompilerServices;
using Microsoft.Extensions.AI;

namespace AutoComplete.Services;

public class SimulatedChatClient : IChatClient
{
    public ChatClientMetadata Metadata => new("Simulated", new Uri("http://localhost"));

    public TService? GetService<TService>(object? key = null) where TService : class => this as TService;

    public async Task<ChatCompletion> CompleteAsync(IList<ChatMessage> chatMessages, ChatOptions? options = null, CancellationToken cancellationToken = default)
    {
        var lastMessage = chatMessages.LastOrDefault();
        var prompt = lastMessage?.Text ?? "No prompt";

        // Real usecase simulation: provide autocomplete suggestions
        var suggestions = $"Here are some suggestions based on '{prompt}':\n1. {prompt} extended\n2. {prompt} refined\n3. {prompt} alternative";

        return new ChatCompletion(new[] { new ChatMessage(ChatRole.Assistant, suggestions) });
    }

    public async IAsyncEnumerable<StreamingChatCompletionUpdate> CompleteStreamingAsync(IList<ChatMessage> chatMessages, ChatOptions? options = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
         var lastMessage = chatMessages.LastOrDefault();
        var prompt = lastMessage?.Text ?? "No prompt";

        yield return new StreamingChatCompletionUpdate { Text = "Simulated " };
        await Task.Delay(10, cancellationToken);
        yield return new StreamingChatCompletionUpdate { Text = "Suggestion " };
        await Task.Delay(10, cancellationToken);
        yield return new StreamingChatCompletionUpdate { Text = $"for: {prompt}" };
    }

    public void Dispose()
    {
        // No-op
    }
}
