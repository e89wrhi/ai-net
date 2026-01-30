using Mapster;
using System.Reflection;

namespace ImageCaption.Features;

using Features.GenerateCaption.V1;
using Features.UploadImage.V1;
using MassTransit;
using Models;
using System.Reflection;
using ImageDto = Dtos.ImageDto;

public class ImageMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Models.Image, ImageDto>()
            .ConstructUsing(x => new ImageDto(x.Id, x.ImageNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.ImageDate, x.Status, x.Price));

        config.NewConfig<CreateImageMongo, ImageReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.ImageId, s => s.Id);

        config.NewConfig<Models.Image, ImageReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.ImageId, s => s.Id.Value);

        config.NewConfig<ImageReadModel, ImageDto>()
            .Map(d => d.Id, s => s.ImageId);

        config.NewConfig<UpdateImageMongo, ImageReadModel>()
            .Map(d => d.ImageId, s => s.Id);

        config.NewConfig<DeleteImageMongo, ImageReadModel>()
            .Map(d => d.ImageId, s => s.Id);

        config.NewConfig<CreateImageRequestDto, CreateImage>()
            .ConstructUsing(x => new CreateImage(x.ImageNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate, x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.ImageDate, x.Status, x.Price));

        config.NewConfig<UpdateImageRequestDto, UpdateImage>()
            .ConstructUsing(x => new UpdateImage(x.Id, x.ImageNumber, x.AircraftId, x.DepartureAirportId, x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.ImageDate, x.Status, x.IsDeleted, x.Price));

    }
}