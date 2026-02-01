using Mapster;
using System.Reflection;

namespace ImageEdit.Features;

using Features.DeleteImageEdit.V1;
using Features.SendImageEdit.V1;
using Features.GetImageEditHistory.V1;
using Features.StartImageEdit.V1;
using MassTransit;
using Models;
using System.Reflection;
using ImageEditDto = Dtos.ImageEditDto;

public class ImageEditMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Models.ImageEdit, ImageEditDto>()
            .ConstructUsing(x => new ImageEditDto(x.Id, x.ImageEditNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.ImageEditDate, x.Status, x.Price));

        config.NewConfig<CreateImageEditMongo, ImageEditSessionReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.ImageEditId, s => s.Id);

        config.NewConfig<Models.ImageEdit, ImageEditSessionReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.ImageEditId, s => s.Id.Value);

        config.NewConfig<ImageEditSessionReadModel, ImageEditDto>()
            .Map(d => d.Id, s => s.ImageEditId);

        config.NewConfig<UpdateImageEditMongo, ImageEditSessionReadModel>()
            .Map(d => d.ImageEditId, s => s.Id);

        config.NewConfig<EditImageMongo, ImageEditSessionReadModel>()
            .Map(d => d.ImageEditId, s => s.Id);

        config.NewConfig<CreateImageEditRequestDto, CreateImageEdit>()
            .ConstructUsing(x => new CreateImageEdit(x.ImageEditNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate, x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.ImageEditDate, x.Status, x.Price));

        config.NewConfig<UpdateImageEditRequestDto, UpdateImageEdit>()
            .ConstructUsing(x => new UpdateImageEdit(x.Id, x.ImageEditNumber, x.AircraftId, x.DepartureAirportId, x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.ImageEditDate, x.Status, x.IsDeleted, x.Price));

    }
}