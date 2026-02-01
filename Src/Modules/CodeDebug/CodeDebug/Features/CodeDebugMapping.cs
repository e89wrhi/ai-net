using Mapster;
using System.Reflection;

namespace CodeDebug.Features;

using Features.DeleteCodeDebug.V1;
using Features.SendCodeDebug.V1;
using Features.GetCodeDebugHistory.V1;
using Features.StartCodeDebug.V1;
using MassTransit;
using Models;
using System.Reflection;
using CodeDebugDto = Dtos.CodeDebugDto;

public class CodeDebugMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Models.CodeDebug, CodeDebugDto>()
            .ConstructUsing(x => new CodeDebugDto(x.Id, x.CodeDebugNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.CodeDebugDate, x.Status, x.Price));

        config.NewConfig<CreateCodeDebugMongo, CodeDebugSessionReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.CodeDebugId, s => s.Id);

        config.NewConfig<Models.CodeDebug, CodeDebugSessionReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.CodeDebugId, s => s.Id.Value);

        config.NewConfig<CodeDebugSessionReadModel, CodeDebugDto>()
            .Map(d => d.Id, s => s.CodeDebugId);

        config.NewConfig<UpdateCodeDebugMongo, CodeDebugSessionReadModel>()
            .Map(d => d.CodeDebugId, s => s.Id);

        config.NewConfig<DebugCodeMongo, CodeDebugSessionReadModel>()
            .Map(d => d.CodeDebugId, s => s.Id);

        config.NewConfig<CreateCodeDebugRequestDto, CreateCodeDebug>()
            .ConstructUsing(x => new CreateCodeDebug(x.CodeDebugNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate, x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.CodeDebugDate, x.Status, x.Price));

        config.NewConfig<UpdateCodeDebugRequestDto, UpdateCodeDebug>()
            .ConstructUsing(x => new UpdateCodeDebug(x.Id, x.CodeDebugNumber, x.AircraftId, x.DepartureAirportId, x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.CodeDebugDate, x.Status, x.IsDeleted, x.Price));

    }
}