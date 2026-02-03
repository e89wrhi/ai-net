using AI.Common.Caching;
using AI.Common.Core;
using Payment.Dtos;

namespace Payment.Features.GetInvoices.V1;

public record GetInvoices(Guid SubscriptionId, Guid UserId) : IQuery<GetInvoicesResult>, ICacheRequest
{
    public string CacheKey => $"GetInvoices_{SubscriptionId}";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

public record GetInvoicesResult(IEnumerable<InvoiceDto> InvoiceDtos);

public record GetInvoicesResponseDto(IEnumerable<InvoiceDto> InvoiceDtos);
