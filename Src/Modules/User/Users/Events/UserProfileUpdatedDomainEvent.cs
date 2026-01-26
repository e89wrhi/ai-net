using AI.Common.Core;
using User.ValueObjects;

namespace User.Events;

public record UserProfileUpdatedDomainEvent(UserId UserId, string FullName, string JobTitle) : IDomainEvent;
