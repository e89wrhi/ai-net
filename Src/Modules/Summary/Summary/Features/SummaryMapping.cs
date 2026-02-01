using Mapster;
using System.Reflection;

namespace Summary.Features;

using Features.DeleteSummary.V1;
using Features.SendSummary.V1;
using Features.GetSummaryHistory.V1;
using Features.StartSummary.V1;
using MassTransit;
using Models;
using System.Reflection;
using SummaryDto = Dtos.SummaryDto;

public class SummaryMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Models.Summary, SummaryDto>()
            .ConstructUsing(x => new SummaryDto(x.Id, x.SummaryNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.SummaryDate, x.Status, x.Price));

        config.NewConfig<CreateSummaryMongo, TextSummaryReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.SummaryId, s => s.Id);

        config.NewConfig<Models.Summary, TextSummaryReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.SummaryId, s => s.Id.Value);

        config.NewConfig<TextSummaryReadModel, SummaryDto>()
            .Map(d => d.Id, s => s.SummaryId);

        config.NewConfig<UpdateSummaryMongo, TextSummaryReadModel>()
            .Map(d => d.SummaryId, s => s.Id);

        config.NewConfig<SummarizeTextMongo, TextSummaryReadModel>()
            .Map(d => d.SummaryId, s => s.Id);

        config.NewConfig<CreateSummaryRequestDto, CreateSummary>()
            .ConstructUsing(x => new CreateSummary(x.SummaryNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate, x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.SummaryDate, x.Status, x.Price));

        config.NewConfig<UpdateSummaryRequestDto, UpdateSummary>()
            .ConstructUsing(x => new UpdateSummary(x.Id, x.SummaryNumber, x.AircraftId, x.DepartureAirportId, x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.SummaryDate, x.Status, x.IsDeleted, x.Price));

    }
}