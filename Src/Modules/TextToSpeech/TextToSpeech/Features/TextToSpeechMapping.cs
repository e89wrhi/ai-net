using Mapster;
using System.Reflection;

namespace TextToSpeech.Features;

using Features.DeleteTextToSpeech.V1;
using Features.SendTextToSpeech.V1;
using Features.GetTextToSpeechHistory.V1;
using Features.StartTextToSpeech.V1;
using MassTransit;
using Models;
using System.Reflection;
using TextToSpeechDto = Dtos.TextToSpeechDto;

public class TextToSpeechMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Models.TextToSpeech, TextToSpeechDto>()
            .ConstructUsing(x => new TextToSpeechDto(x.Id, x.TextToSpeechNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.TextToSpeechDate, x.Status, x.Price));

        config.NewConfig<CreateTextToSpeechMongo, TextToSpeechSessionReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.TextToSpeechId, s => s.Id);

        config.NewConfig<Models.TextToSpeech, TextToSpeechSessionReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.TextToSpeechId, s => s.Id.Value);

        config.NewConfig<TextToSpeechSessionReadModel, TextToSpeechDto>()
            .Map(d => d.Id, s => s.TextToSpeechId);

        config.NewConfig<UpdateTextToSpeechMongo, TextToSpeechSessionReadModel>()
            .Map(d => d.TextToSpeechId, s => s.Id);

        config.NewConfig<GenerateSpeechMongo, TextToSpeechSessionReadModel>()
            .Map(d => d.TextToSpeechId, s => s.Id);

        config.NewConfig<CreateTextToSpeechRequestDto, CreateTextToSpeech>()
            .ConstructUsing(x => new CreateTextToSpeech(x.TextToSpeechNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate, x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.TextToSpeechDate, x.Status, x.Price));

        config.NewConfig<UpdateTextToSpeechRequestDto, UpdateTextToSpeech>()
            .ConstructUsing(x => new UpdateTextToSpeech(x.Id, x.TextToSpeechNumber, x.AircraftId, x.DepartureAirportId, x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.TextToSpeechDate, x.Status, x.IsDeleted, x.Price));

    }
}