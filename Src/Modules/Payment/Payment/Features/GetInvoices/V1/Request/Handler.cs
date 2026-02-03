using AI.Common.Core;
using Ardalis.GuardClauses;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Payment.Data;
using Payment.Dtos;
using Payment.Exceptions;

namespace Payment.Features.GetInvoices.V1;

internal class GetInvoicesHandler : IQueryHandler<GetInvoices, GetInvoicesResult>
{
    private readonly IMapper _mapper;
    private readonly PaymentDbContext _dbContext;

    public GetInvoicesHandler(IMapper mapper, PaymentDbContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public async Task<GetInvoicesResult> Handle(GetInvoices request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var subscription = await _dbContext.Subscriptions
            .Include(x => x.Invoices)
            .FirstOrDefaultAsync(x => x.Id == request.SubscriptionId && x.UserId == request.UserId, cancellationToken);

        if (subscription == null)
        {
            throw new SubscriptionNotFoundException(request.SubscriptionId);
        }

        var dtos = _mapper.Map<IEnumerable<InvoiceDto>>(subscription.Invoices);

        return new GetInvoicesResult(dtos);
    }
}


