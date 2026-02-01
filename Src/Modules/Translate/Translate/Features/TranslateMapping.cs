using Mapster;
using System.Reflection;

namespace Translate.Features;

using Features.DeleteTranslate.V1;
using Features.SendTranslate.V1;
using Features.GetTranslateHistory.V1;
using Features.StartTranslate.V1;
using MassTransit;
using Models;
using System.Reflection;
using TranslateDto = Dtos.TranslateDto;

public class TranslateMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Models.Translate, TranslateDto>()
            .ConstructUsing(x => new TranslateDto(x.Id, x.TranslateNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.TranslateDate, x.Status, x.Price));

        config.NewConfig<CreateTranslateMongo, TranslationSessionReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.TranslateId, s => s.Id);

        config.NewConfig<Models.Translate, TranslationSessionReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.TranslateId, s => s.Id.Value);

        config.NewConfig<TranslationSessionReadModel, TranslateDto>()
            .Map(d => d.Id, s => s.TranslateId);

        config.NewConfig<UpdateTranslateMongo, TranslationSessionReadModel>()
            .Map(d => d.TranslateId, s => s.Id);

        config.NewConfig<TranslateTextMongo, TranslationSessionReadModel>()
            .Map(d => d.TranslateId, s => s.Id);

        config.NewConfig<CreateTranslateRequestDto, CreateTranslate>()
            .ConstructUsing(x => new CreateTranslate(x.TranslateNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate, x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.TranslateDate, x.Status, x.Price));

        config.NewConfig<UpdateTranslateRequestDto, UpdateTranslate>()
            .ConstructUsing(x => new UpdateTranslate(x.Id, x.TranslateNumber, x.AircraftId, x.DepartureAirportId, x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.TranslateDate, x.Status, x.IsDeleted, x.Price));

    }
}