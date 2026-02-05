using AI.Common.Core;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using User.Data;
using User.Models;
using User.ValueObjects;

namespace User.Features.CreateUser.V1;

internal class CreateUserHandler : ICommandHandler<CreateUserCommand, CreateUserCommandResponse>
{
    private readonly UserDbContext _dbContext;

    public CreateUserHandler(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreateUserCommandResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var userId = UserId.Of(request.UserId);
        
        var existingAnalytics = await _dbContext.UserAnalytics
            .FirstOrDefaultAsync(x => x.User == userId, cancellationToken);
            
        if (existingAnalytics != null)
        {
            return new CreateUserCommandResponse(existingAnalytics.Id.Value);
        }
        
        var analytics = UserAnalytics.Create(UserAnalyticsId.Of(NewId.NextGuid()), userId);
        _dbContext.UserAnalytics.Add(analytics);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new CreateUserCommandResponse(analytics.Id.Value);
    }
}
