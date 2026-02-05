using AI.Common.Core;
using Ardalis.GuardClauses;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Payment.Data;
using Payment.Dtos;
using Payment.Enums;
using Payment.Exceptions;
using Payment.ValueObjects;

namespace Payment.Features.GetSubscription.V1;

internal class GetSubscriptionHandler : IQueryHandler<GetSubscription, GetSubscriptionResult>
{
    private readonly IMapper _mapper;
    private readonly PaymentDbContext _dbContext;

    public GetSubscriptionHandler(IMapper mapper, PaymentDbContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public async Task<GetSubscriptionResult> Handle(GetSubscription request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var subscription = await _dbContext.Subscriptions
            .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.Status == Enums.SubscriptionStatus.Active, cancellationToken);

        if (subscription == null)
        {
            throw new SubscriptionNotFoundException(request.UserId);
        }

        var dto = _mapper.Map<SubscriptionDto>(subscription);

        return new GetSubscriptionResult(dto);
    }
}


