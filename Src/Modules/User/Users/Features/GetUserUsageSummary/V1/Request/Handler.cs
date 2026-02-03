using AI.Common.Core;
using Ardalis.GuardClauses;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using User.Data;
using User.Dtos;
using User.Exceptions;

namespace User.Features.GetUserUsageSummary.V1;

internal class GetUserUsageSummaryHandler : IQueryHandler<GetUserUsageSummary, GetUserUsageSummaryResult>
{
    private readonly IMapper _mapper;
    private readonly UserDbContext _dbContext;

    public GetUserUsageSummaryHandler(IMapper mapper, UserDbContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public async Task<GetUserUsageSummaryResult> Handle(GetUserUsageSummary request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var analytics = await _dbContext.UserAnalytics
            .FirstOrDefaultAsync(x => x.User.Value == request.UserId, cancellationToken);

        if (analytics == null)
        {
            throw new UserNotFoundException(request.UserId);
        }

        // Map from SQL analytics model to DTOs
        // Here we create a summary based on the multiple request counters in the analytics model
        var usageDtos = new List<UserUsageSummaryDto>
        {
            new() { Id = Guid.NewGuid(), Period = "Today", RequestsCount = (int)analytics.TodayRequests, TokenUsed = "N/A" },
            new() { Id = Guid.NewGuid(), Period = "This Week", RequestsCount = (int)analytics.WeekRequests, TokenUsed = "N/A" },
            new() { Id = Guid.NewGuid(), Period = "This Month", RequestsCount = (int)analytics.MonthRequests, TokenUsed = "N/A" },
            new() { Id = Guid.NewGuid(), Period = "Total", RequestsCount = (int)analytics.TotalRequests, TokenUsed = "N/A" }
        };

        return new GetUserUsageSummaryResult(usageDtos);
    }
}

