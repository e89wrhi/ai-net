using AI.Common.Core;
using Payment.ValueObjects;

namespace Payment.Models;

public record SubscriptionModel : Aggregate<SubscriptionId>
{
        SubscriptionId
   UserId
  Plan
  Status
  StartedAt
  RenewedAt
  ExpiresAt
    }
