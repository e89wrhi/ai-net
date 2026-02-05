using AI.Common.Core;
using Ardalis.GuardClauses;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using User.Data;
using User.Dtos;

namespace User.Features.GetUserActivity.V1;

internal class GetUserActivityHandler : IQueryHandler<GetUserActivity, GetUserActivityResult>
{
    private readonly IMapper _mapper;
    private readonly UserDbContext _dbContext;

    public GetUserActivityHandler(IMapper mapper, UserDbContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public async Task<GetUserActivityResult> Handle(GetUserActivity request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var activities = await _dbContext.Sessions
            .Include(x => x.Actions)
            .Where(x => x.UserId.Value == request.UserId)
            .OrderByDescending(x => x.LastActivityAt)
            .ToListAsync(cancellationToken);

        // Flatten sessions into activity DTOs
        var activityDtos = activities.SelectMany(s => s.Actions.Select(a => new UserActivityDto
        {
            Id = a.Id,
            Module = "Internal", // This could be mapped from session if available
            Action = a.ActionType,
            TimeStamp = a.PerformedAt,
            ResourceId = s.Id
        })).ToList();

        return new GetUserActivityResult(activityDtos);
    }
}

