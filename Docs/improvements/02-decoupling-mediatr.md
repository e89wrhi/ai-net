# 🔗 In-Process Coupling & Latency Bubbles

## Overview
A critical danger in the Modular Monolith is the temptation to communicate between modules synchronously via `IMediator.Send()`. For example, the `Payment` module synchronously querying the `Identity` module to get user details, or `ChatBot` querying `Payment` to ensure the user has sufficient balance before executing an LLM call.

## The Core Problem
When Module A synchronously awaits a response from Module B:
1. **Latency Bubbles**: If Module B is slow (or its database is locked), Module A comes to a dead halt. This cascades through the application, destroying throughput.
2. **Hard Coupling**: It prevents splitting the Monolith into independent microservices in the future.
3. **Transaction Spanning**: A failure in Module B rolls back transactions in Module A, violating Bounded Context integrity.

## The Solution: Eventual Consistency & Local Read-Models

Modules should rarely ask other modules for data. Instead, they should subscribe to **Integration Events** and maintain their own local projections (Read-Models) of the data they care about.

### Implementation Steps

#### 1. Define Integration Events
In your `Common.Contracts` project, define C# records for state changes:

```csharp
namespace AI.Common.Contracts;

public record UserCreditsUpdatedIntegrationEvent(
    Guid UserId, 
    int RemainingCredits
) : IIntegrationEvent;
```

#### 2. Publish Events via the Outbox
When the `Payment` module updates credits, it doesn't notify anyone directly. It simply saves an Outbox message in the same EF Core transaction as the state change.

```csharp
// Inside Payment Feature Handler
user.DeductCredits(10);
await _dbContext.Users.AddAsync(user);

// MassTransit Outbox transparently groups this into the DB transaction
await _publishEndpoint.Publish(new UserCreditsUpdatedIntegrationEvent(user.Id, user.Credits));

await _dbContext.SaveChangesAsync(); 
```

#### 3. Consume & Replicate
The `ChatBot` module (which needs to know user balances before calling the LLM) listens to this event.

```csharp
public class UserCreditsUpdatedEventConsumer : IConsumer<UserCreditsUpdatedIntegrationEvent>
{
    private readonly ChatBotDbContext _dbContext;

    public async Task Consume(ConsumeContext<UserCreditsUpdatedIntegrationEvent> context)
    {
        // Update the ChatBot module's local "subset" of the user data
        var localUserRecord = await _dbContext.UserReadModels.FindAsync(context.Message.UserId);
        localUserRecord.CachedCredits = context.Message.RemainingCredits;
        await _dbContext.SaveChangesAsync();
    }
}
```

#### 4. Fast, Local Queries
Now, when a user tries to generate a ChatBot response, the `ChatBot` module simply queries its *own* database:

```csharp
// Fast, no waiting on the Payment module to respond!
var userContext = await _dbContext.UserReadModels.FindAsync(request.UserId);
if(userContext.CachedCredits < 1) 
{
    throw new InsufficientCreditsException();
}
```

## Exceptions
If extreme real-time consistency is absolutely mandatory (where a 5-second eventual consistency lag is unacceptable), use **internal gRPC calls**. However, default to the local Read-Model pattern 90% of the time.
