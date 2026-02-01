using Mapster;
using System.Reflection;

namespace User.Features;

using Features.CreateUser.V1;
using Features.ResetUsageCounters.V1;
using Features.TrackActivity.V1;
using Features.GetUserActivity.V1;
using Features.GetUserUsageSummary.V1;
using Features.UpdateUser.V1;
using MassTransit;
using Models;
using System.Reflection;
using UserDto = Dtos.UserDto;

public class UserMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Models.UserActivitySession, UserDto>()
            .Map(d => d.Id, s => s.Id.Value.ToString())
            .Map(d => d.HomeTeam, s => s.HomeTeam)
            .Map(d => d.AwayTeam, s => s.AwayTeam)
            .Map(d => d.HomeTeamScore, s => s.HomeTeamScore)
            .Map(d => d.AwayTeamScore, s => s.AwayTeamScore)
            .Map(d => d.League, s => s.League)
            .Map(d => d.Status, s => s.Status)
            .Map(d => d.MatchTime, s => s.MatchTime)
            .Map(d => d.EventsCount, s => s.EventsCount)
            .Map(d => d.HomeVotesCount, s => s.HomeVotesCount)
            .Map(d => d.AwayVotesCount, s => s.AwayVotesCount)
            .Map(d => d.DrawVotesCount, s => s.DrawVotesCount);

        config.NewConfig<CreateUserMongo, UserAnalyticsReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.UserId, s => s.Id);

        config.NewConfig<Models.User, UserAnalyticsReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.UserId, s => s.Id.Value);

        config.NewConfig<UserAnalyticsReadModel, UserDto>()
            .Map(d => d.Id, s => s.UserId);

        config.NewConfig<UpdateUserMongo, UserAnalyticsReadModel>()
            .Map(d => d.UserId, s => s.Id);

        config.NewConfig<DeleteUserMongo, UserAnalyticsReadModel>()
            .Map(d => d.UserId, s => s.Id);

        config.NewConfig<CreateUserRequestDto, CreateUser>()
            .ConstructUsing(x => new CreateUser(x.UserNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate, x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.UserDate, x.Status, x.Price));

        config.NewConfig<UpdateUserRequestDto, UpdateUser>()
            .ConstructUsing(x => new UpdateUser(x.Id, x.UserNumber, x.AircraftId, x.DepartureAirportId, x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.UserDate, x.Status, x.IsDeleted, x.Price));

    }
}