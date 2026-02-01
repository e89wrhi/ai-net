using Mapster;
using System.Reflection;

namespace ImageGen.Features;

using Features.DeleteImageGen.V1;
using Features.SendImageGen.V1;
using Features.GetImageGenHistory.V1;
using Features.StartImageGen.V1;
using MassTransit;
using Models;
using System.Reflection;
using ImageGenDto = Dtos.ImageGenDto;

public class ImageGenMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Models.ImageGen, ImageGenDto>()
            .ConstructUsing(x => new ImageGenDto(x.Id, x.ImageGenNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.ImageGenDate, x.Status, x.Price));

        config.NewConfig<CreateImageGenMongo, ImageGenerationReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.ImageGenId, s => s.Id);

        config.NewConfig<Models.ImageGen, ImageGenerationReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.ImageGenId, s => s.Id.Value);

        config.NewConfig<ImageGenerationReadModel, ImageGenDto>()
            .Map(d => d.Id, s => s.ImageGenId);

        config.NewConfig<UpdateImageGenMongo, ImageGenerationReadModel>()
            .Map(d => d.ImageGenId, s => s.Id);

        config.NewConfig<GenerateImageMongo, ImageGenerationReadModel>()
            .Map(d => d.ImageGenId, s => s.Id);

        config.NewConfig<CreateImageGenRequestDto, CreateImageGen>()
            .ConstructUsing(x => new CreateImageGen(x.ImageGenNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate, x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.ImageGenDate, x.Status, x.Price));

        config.NewConfig<UpdateImageGenRequestDto, UpdateImageGen>()
            .ConstructUsing(x => new UpdateImageGen(x.Id, x.ImageGenNumber, x.AircraftId, x.DepartureAirportId, x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.ImageGenDate, x.Status, x.IsDeleted, x.Price));

    }
}