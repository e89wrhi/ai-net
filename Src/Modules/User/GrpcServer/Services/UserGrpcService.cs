using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using Google.Protobuf.WellKnownTypes;
using User.GrpcServer.Protos;

using Protos = User.GrpcServer.Protos;

namespace User.GrpcServer.Services;

public class UserGrpcService : Protos.UserGrpcService.UserGrpcServiceBase
{
    private readonly IMediator _mediator;

    public UserGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        var cmd = new User.Features.CreateUser.V1.CreateUserCommand(Guid.Parse(request.UserId));
        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new CreateUserResponse { Id = result.Id.ToString() };
    }

    public override async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
    {
        var cmd = new User.Features.UpdateUser.V1.UpdateUserCommand(
            Guid.Parse(request.SessionId),
            request.FullName,
            request.Email);
        
        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new UpdateUserResponse { Success = result.Success };
    }

    public override async Task<GetUserActivityResponse> GetUserActivity(GetUserActivityRequest request, ServerCallContext context)
    {
        var query = new User.Features.GetUserActivity.V1.GetUserActivity(Guid.Parse(request.UserId));
        var result = await _mediator.Send(query, context.CancellationToken);

        var response = new GetUserActivityResponse();

        foreach (var dto in result.UserActivityDtos)
        {
            var activity = new UserActivity
            {
                Id = dto.Id.ToString(),
                Module = dto.Module,
                Action = dto.Action,
                ResourceId = dto.ResourceId.ToString()
            };

            var utc = DateTime.SpecifyKind(dto.TimeStamp.UtcDateTime, DateTimeKind.Utc);
            activity.Timestamp = Timestamp.FromDateTime(utc);

            response.Activities.Add(activity);
        }

        return response;
    }

    public override async Task<GetUserUsageSummaryResponse> GetUserUsageSummary(GetUserUsageSummaryRequest request, ServerCallContext context)
    {
        var query = new User.Features.GetUserUsageSummary.V1.GetUserUsageSummary(Guid.Parse(request.UserId));
        var result = await _mediator.Send(query, context.CancellationToken);

        var response = new GetUserUsageSummaryResponse();

        foreach (var dto in result.UserUsageSummaryDtos)
        {
            response.Usages.Add(new UserUsage
            {
                Id = dto.Id.ToString(),
                Period = dto.Period,
                TokenUsed = dto.TokenUsed,
                RequestsCount = dto.RequestsCount
            });
        }

        return response;
    }

    public override async Task<ResetUsageCountersResponse> ResetUsageCounters(ResetUsageCountersRequest request, ServerCallContext context)
    {
        var cmd = new User.Features.ResetUsageCounters.V1.ResetUsageCounterCommand(Guid.Parse(request.UserId));
        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new ResetUsageCountersResponse { Id = result.Id.ToString() };
    }

    public override async Task<TrackActivityResponse> TrackActivity(TrackActivityRequest request, ServerCallContext context)
    {
        var cmd = new User.Features.TrackActivity.V1.TrackActivityCommand(
            Guid.Parse(request.UserId),
            (User.Enums.TrackedModule)request.Module,
            request.Action,
            Guid.Parse(request.ResourceId),
            request.IpAddress,
            request.UserAgent);

        var result = await _mediator.Send(cmd, context.CancellationToken);

        return new TrackActivityResponse { Id = result.Id.ToString() };
    }
}

