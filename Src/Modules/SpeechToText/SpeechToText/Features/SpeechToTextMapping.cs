using Mapster;
using System.Reflection;

namespace SpeechToText.Features;

using Features.DeleteSpeechToText.V1;
using Features.SendSpeechToText.V1;
using Features.GetSpeechToTextHistory.V1;
using Features.StartSpeechToText.V1;
using MassTransit;
using Models;
using System.Reflection;
using SpeechToTextDto = Dtos.SpeechToTextDto;

public class SpeechToTextMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Models.SpeechToText, SpeechToTextDto>()
            .ConstructUsing(x => new SpeechToTextDto(x.Id, x.SpeechToTextNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.SpeechToTextDate, x.Status, x.Price));

        config.NewConfig<CreateSpeechToTextMongo, SpeechToTextSessionReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.SpeechToTextId, s => s.Id);

        config.NewConfig<Models.SpeechToText, SpeechToTextSessionReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.SpeechToTextId, s => s.Id.Value);

        config.NewConfig<SpeechToTextSessionReadModel, SpeechToTextDto>()
            .Map(d => d.Id, s => s.SpeechToTextId);

        config.NewConfig<UpdateSpeechToTextMongo, SpeechToTextSessionReadModel>()
            .Map(d => d.SpeechToTextId, s => s.Id);

        config.NewConfig<UploadSpeechAudioMongo, SpeechToTextSessionReadModel>()
            .Map(d => d.SpeechToTextId, s => s.Id);

        config.NewConfig<CreateSpeechToTextRequestDto, CreateSpeechToText>()
            .ConstructUsing(x => new CreateSpeechToText(x.SpeechToTextNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate, x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.SpeechToTextDate, x.Status, x.Price));

        config.NewConfig<UpdateSpeechToTextRequestDto, UpdateSpeechToText>()
            .ConstructUsing(x => new UpdateSpeechToText(x.Id, x.SpeechToTextNumber, x.AircraftId, x.DepartureAirportId, x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.SpeechToTextDate, x.Status, x.IsDeleted, x.Price));

    }
}