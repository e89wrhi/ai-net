using AI.Common.Core;
using User.ValueObjects;

namespace User.Events;

public record UserCreatedDomainEvent(UserId UserId, string Email, string Username) : IDomainEvent;
