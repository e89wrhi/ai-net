using MediatR;
using User.Data;

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

        var user = await _dbContext.Users.FindAsync(new object[] { UserId.Of(request.UserId) }, cancellationToken);

        if (user == null)
        {
            throw new UserNotFoundException(request.UserId);
        }

        user.ResetUsages();

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ResetUsageCounterCommandResponse(user.Id.Value);
    }
}

