using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using User.Data;

namespace User.GrpcServer.Services;

public class UserGrpcService : User.UserGrpcService.UserGrpcServiceBase
{
    private readonly IMediator _mediator;
    private readonly UserDbContext _dbContext;

    public UserGrpcService(IMediator mediator, UserDbContext dbContext)
    {
        _mediator = mediator;
        _dbContext = dbContext;
    }

    public override async Task<GetUserResponse> GetUser(GetUserRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.UserId, out var id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid UserId format"));
        }

        var userId = ValueObjects.UserId.Of(id);

        // Fetch analytics
        var analytics = await _dbContext.UserAnalytics
            .FirstOrDefaultAsync(x => x.User == userId, context.CancellationToken);

        // Fetch sessions
        var sessions = await _dbContext.Sessions
            .Include(x => x.Actions)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.LastActivityAt)
            .Take(10) // Limit for Grpc
            .ToListAsync(context.CancellationToken);

        var response = new GetUserResponse
        {
            Id = id.ToString(),
            Username = "User_" + id.ToString().Substring(0, 8), // Placeholder as User entity is in Identity module
            Email = string.Empty,
            FullName = "AI User"
        };

        foreach (var session in sessions)
        {
            foreach (var a in session.Actions)
            {
                var act = new UserActivity
                {
                    Id = a.Id.Value.ToString(),
                    Module = "Internal",
                    Action = a.ActionType,
                    ResourceId = session.Id.Value.ToString()
                };

                var utc = DateTime.SpecifyKind(a.PerformedAt.ToUniversalTime(), DateTimeKind.Utc);
                act.Timestamp = Timestamp.FromDateTime(utc);

                response.Activities.Add(act);
            }
        }

        if (analytics != null)
        {
            response.Usages.Add(new UserUsage
            {
                Id = analytics.Id.Value.ToString(),
                Period = "Lifetime",
                TokenUsed = "N/A",
                RequestsCount = (int)analytics.TotalRequests
            });
            
            response.Usages.Add(new UserUsage
            {
                Id = analytics.Id.Value.ToString() + "_today",
                Period = "Today",
                RequestsCount = (int)analytics.TodayRequests
            });
        }

        return response;
    }
}
