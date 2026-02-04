using System.Runtime.CompilerServices;
using Microsoft.Extensions.AI;

namespace CodeDebug.Services;

public class SimulatedChatClient : IChatClient
{
    public ChatClientMetadata Metadata => new("Simulated", new Uri("http://localhost"));

    public object? GetService(Type serviceType, object? serviceKey = null) =>
        serviceKey is not null ? null :
        serviceType == typeof(ChatClientMetadata) ? Metadata :
        serviceType?.IsInstanceOfType(this) is true ? this :
        null;

    public async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> chatMessages, 
        ChatOptions? options = null, 
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(50, cancellationToken);
        var messages = chatMessages.ToList();
        var systemMessage = messages.FirstOrDefault(m => m.Role == ChatRole.System)?.Text ?? "";

        string responseText = systemMessage.Contains("JSON") 
            ? "{\"summary\": \"Simulated analysis: Potential issue found in the code structure.\", \"issueCount\": 1}" 
            : "Simulated response message.";

        return new ChatResponse(new[] { new ChatMessage(ChatRole.Assistant, responseText) });
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
