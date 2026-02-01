using Mapster;
using System.Reflection;

namespace Resume.Features;

using Features.AnalyzeResume.V1;
using Features.ReAnalyzeResume.V1;
using MassTransit;
using Models;
using System.Reflection;
using Features.UploadResume.V1;
using Features.GetResumeAnalysis.V1;
using Features.GetResumeSuggestions.V1;
using ResumeDto = Dtos.ResumeAnalysisDto;

public class ResumeMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Models.ResumeModel, ResumeDto>()
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

        config.NewConfig<CreateResumeMongo, ResumeAnalysisReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.ResumeId, s => s.Id);

        config.NewConfig<Models.Resume, ResumeAnalysisReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.ResumeId, s => s.Id.Value);

        config.NewConfig<ResumeAnalysisReadModel, ResumeDto>()
            .Map(d => d.Id, s => s.ResumeId);

        config.NewConfig<UpdateResumeMongo, ResumeAnalysisReadModel>()
            .Map(d => d.ResumeId, s => s.Id);

        config.NewConfig<DeleteResumeMongo, ResumeAnalysisReadModel>()
            .Map(d => d.ResumeId, s => s.Id);

        config.NewConfig<CreateResumeRequestDto, CreateResume>()
            .ConstructUsing(x => new CreateResume(x.ResumeNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate, x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.ResumeDate, x.Status, x.Price));

        config.NewConfig<UpdateResumeRequestDto, UpdateResume>()
            .ConstructUsing(x => new UpdateResume(x.Id, x.ResumeNumber, x.AircraftId, x.DepartureAirportId, x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.ResumeDate, x.Status, x.IsDeleted, x.Price));

    }
}