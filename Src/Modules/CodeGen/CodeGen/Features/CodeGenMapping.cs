using Mapster;
using System.Reflection;

namespace CodeGen.Features;

using Features.DeleteCodeGen.V1;
using Features.SendCodeGen.V1;
using Features.GetCodeGenHistory.V1;
using Features.StartCodeGen.V1;
using MassTransit;
using Models;
using System.Reflection;
using CodeGenDto = Dtos.CodeGenDto;

public class CodeGenMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Models.CodeGen, CodeGenDto>()
            .ConstructUsing(x => new CodeGenDto(x.Id, x.CodeGenNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.CodeGenDate, x.Status, x.Price));

        config.NewConfig<CreateCodeGenMongo, CodeGenerationReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.CodeGenId, s => s.Id);

        config.NewConfig<Models.CodeGen, CodeGenerationReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.CodeGenId, s => s.Id.Value);

        config.NewConfig<CodeGenerationReadModel, CodeGenDto>()
            .Map(d => d.Id, s => s.CodeGenId);

        config.NewConfig<UpdateCodeGenMongo, CodeGenerationReadModel>()
            .Map(d => d.CodeGenId, s => s.Id);

        config.NewConfig<GenerateCodeMongo, CodeGenerationReadModel>()
            .Map(d => d.CodeGenId, s => s.Id);

        config.NewConfig<CreateCodeGenRequestDto, CreateCodeGen>()
            .ConstructUsing(x => new CreateCodeGen(x.CodeGenNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate, x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.CodeGenDate, x.Status, x.Price));

        config.NewConfig<UpdateCodeGenRequestDto, UpdateCodeGen>()
            .ConstructUsing(x => new UpdateCodeGen(x.Id, x.CodeGenNumber, x.AircraftId, x.DepartureAirportId, x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.CodeGenDate, x.Status, x.IsDeleted, x.Price));

    }
}