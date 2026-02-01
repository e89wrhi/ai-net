using Mapster;
using System.Reflection;

namespace Sentiment.Features;

using Features.DeleteSentiment.V1;
using Features.SendSentiment.V1;
using Features.GetSentimentHistory.V1;
using Features.StartSentiment.V1;
using MassTransit;
using Models;
using System.Reflection;
using SentimentDto = Dtos.SentimentDto;

public class SentimentMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Models.Sentiment, SentimentDto>()
            .ConstructUsing(x => new SentimentDto(x.Id, x.SentimentNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.SentimentDate, x.Status, x.Price));

        config.NewConfig<CreateSentimentMongo, TextSentimentReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.SentimentId, s => s.Id);

        config.NewConfig<Models.Sentiment, TextSentimentReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.SentimentId, s => s.Id.Value);

        config.NewConfig<TextSentimentReadModel, SentimentDto>()
            .Map(d => d.Id, s => s.SentimentId);

        config.NewConfig<UpdateSentimentMongo, TextSentimentReadModel>()
            .Map(d => d.SentimentId, s => s.Id);

        config.NewConfig<GenerateSentimentMongo, TextSentimentReadModel>()
            .Map(d => d.SentimentId, s => s.Id);

        config.NewConfig<CreateSentimentRequestDto, CreateSentiment>()
            .ConstructUsing(x => new CreateSentiment(x.SentimentNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate, x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.SentimentDate, x.Status, x.Price));

        config.NewConfig<UpdateSentimentRequestDto, UpdateSentiment>()
            .ConstructUsing(x => new UpdateSentiment(x.Id, x.SentimentNumber, x.AircraftId, x.DepartureAirportId, x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.SentimentDate, x.Status, x.IsDeleted, x.Price));

    }
}