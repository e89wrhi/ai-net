using AI.Common.Core;
using Payment.ValueObjects;

namespace Payment.Models;

public record InvoiceModel : Entity<InvoiceId>
{
        InvoiceId
Period
TotalAmount
LineItems
Status
    }
