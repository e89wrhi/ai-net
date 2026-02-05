using AI.Common.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using User.Data;
using User.ValueObjects;
using User.Exceptions;
using Ardalis.GuardClauses;

namespace User.Features.ResetUsageCounters.V1;


internal class ResetUsageCounterHandler : IRequestHandler<ResetUsageCounterCommand, ResetUsageCounterCommandResponse>
{
    private readonly UserDbContext _dbContext;

    public ResetUsageCounterHandler(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ResetUsageCounterCommandResponse> Handle(ResetUsageCounterCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var userId = UserId.Of(request.UserId);
        var analytics = await _dbContext.UserAnalytics
            .FirstOrDefaultAsync(x => x.User == userId, cancellationToken);

        if (analytics == null)
        {
            throw new UserNotFoundException(request.UserId);
        }

        analytics.ResetDaily();
        analytics.ResetWeekly();
        analytics.ResetMonthly();

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ResetUsageCounterCommandResponse(analytics.Id.Value);
    }
}

