using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Google.Protobuf.WellKnownTypes;
using User.Data;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace User.GrpcServer.Services;

public class UserGrpcService : User.UserGrpcService.UserGrpcServiceBase
{
    private readonly IMediator _mediator;
    private readonly UserReadDbContext _readDbContext;

    public UserGrpcService(IMediator mediator, UserReadDbContext readDbContext)
    {
        _mediator = mediator;
        _readDbContext = readDbContext;
    }

    public override async Task<GetUserResponse> GetUser(GetUserRequest request, ServerCallContext context)
    {
        var id = Guid.Parse(request.UserId);

        var user = await _readDbContext.User.AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == id, context.CancellationToken);

        if (user == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"User '{request.UserId}' not found"));
        }

        var response = new GetUserResponse
        {
            Id = user.Id.ToString(),
            Username = user.Username ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName ?? string.Empty
        };

        foreach (var a in user.Activities)
        {
            var act = new UserActivity
            {
                Id = a.Id.ToString(),
                Module = a.Module ?? string.Empty,
                Action = a.Action ?? string.Empty,
                ResourceId = a.ResourceId.ToString()
            };

            if (a.TimeStamp != default)
            {
                var utc = DateTime.SpecifyKind(a.TimeStamp.ToUniversalTime().DateTime, DateTimeKind.Utc);
                act.Timestamp = Timestamp.FromDateTime(utc);
            }

            response.Activities.Add(act);
        }

        foreach (var u in user.Usages)
        {
            var usage = new UserUsage
            {
                Id = u.Id.ToString(),
                Period = u.Period ?? string.Empty,
                TokenUsed = u.TokenUsed ?? string.Empty,
                RequestsCount = u.RequestsCount
            };

            response.Usages.Add(usage);
        }

        return response;
    }
}
