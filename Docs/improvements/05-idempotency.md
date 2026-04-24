# 🔄 Idempotency Protections

## Overview
AI capabilities, particularly Large Language Model (LLM) invocations, are uniquely challenging. They are high-latency (taking several seconds or minutes to resolve) and computationally expensive.

## The Core Problem
Because APIs can be slow, clients (or end-users) often aggressively retry requests (e.g., clicking "Generate Code" multiple times because the loading spinner feels stuck). 
If these repeated HTTP requests reach your MediatR handlers:
1. **Duplicate Billing**: The `Payment` module will charge the user for multiple LLM tokens.
2. **Rate Limit Exhaustion**: Your OpenAI/Anthropic quota will be burned.
3. **Ghost Processing**: The background service works endlessly on orphan tasks that the client actually abandoned.

## The Solution: Idempotent CQRS Commands

Every command that mutates state or costs money must carry a client-generated uniqueness identifier (`Idempotency-Key` header). 

### Implementation Steps

#### 1. Require Headers on the Endpoint
Modify `GenerateCodeEndpoint.cs` (and all other mutators) to intercept the key:

```csharp
builder.MapPost($"{EndpointConfig.BaseApiPath}/codegen/generate",
    async ([FromHeader(Name = "Idempotency-Key")] string idempotencyKey, 
           GenerateCodeRequestDto request, 
           IMediator mediator) =>
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
            return Results.BadRequest(new { error = "Idempotency-Key required." });

        var command = new GenerateCodeCommand(idempotencyKey, request.Prompt, /* ... */);
        return await mediator.Send(command);
    });
```

#### 2. Introduce a MediatR Pipeline Behavior
Create a pipeline behavior in `Src/Common/Core` that intercepts commands before they execute.

```csharp
public class IdempotencyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IIdempotentCommand<TResponse>
{
    private readonly IDistributedCache _cache;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var cacheKey = $"idempotency:{request.IdempotencyKey}";
        
        // 1. Check if we already processed this
        var cachedResponse = await _cache.GetStringAsync(cacheKey);
        if (cachedResponse != null)
        {
            return JsonSerializer.Deserialize<TResponse>(cachedResponse);
        }

        // 2. Lock the cache (indicate we are processing) to block parallel duplicate clicks
        await _cache.SetStringAsync(cacheKey, "processing", new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2) });

        // 3. Actually execute the LLM Command
        var response = await next();

        // 4. Save the successful result to short-circuit future duplicate requests
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(response), 
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) });

        return response;
    }
}
```

## Outcome
By enforcing idempotency at the framework level, you guarantee that even if a network partition triggers 10 automatic retries from the client, the AI Orchestration layer only ever fires 1 payload. You successfully protect system uptime and financial budget.
