using Mapster;
using System.Reflection;

namespace Meeting.Features;

using Features.SummarizeMeetingAudio.V1;
using Features.UploadMeetingAudio.V1;
using MassTransit;
using Models;
using System.Reflection;
using Features.GetMeetingSummary.V1;
using MeetingDto = Dtos.MeetingSummaryDto;

public class MeetingMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Models.MeetingModel, MeetingDto>()
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

        config.NewConfig<CreateMeetingMongo, MeetingAnalysisSessionReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.MeetingId, s => s.Id);

        config.NewConfig<Models.Meeting, MeetingAnalysisSessionReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.MeetingId, s => s.Id.Value);

        config.NewConfig<MeetingAnalysisSessionReadModel, MeetingDto>()
            .Map(d => d.Id, s => s.MeetingId);

        config.NewConfig<UpdateMeetingMongo, MeetingAnalysisSessionReadModel>()
            .Map(d => d.MeetingId, s => s.Id);

        config.NewConfig<DeleteMeetingMongo, MeetingAnalysisSessionReadModel>()
            .Map(d => d.MeetingId, s => s.Id);

        config.NewConfig<CreateMeetingRequestDto, CreateMeeting>()
            .ConstructUsing(x => new CreateMeeting(x.MeetingNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate, x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.MeetingDate, x.Status, x.Price));

        config.NewConfig<UpdateMeetingRequestDto, UpdateMeeting>()
            .ConstructUsing(x => new UpdateMeeting(x.Id, x.MeetingNumber, x.AircraftId, x.DepartureAirportId, x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.MeetingDate, x.Status, x.IsDeleted, x.Price));

    }
}