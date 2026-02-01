using Mapster;
using System.Reflection;

namespace AutoComplete.Features;

using Features.DeleteAutoComplete.V1;
using Features.SendAutoComplete.V1;
using Features.GetAutoCompleteHistory.V1;
using Features.StartAutoComplete.V1;
using MassTransit;
using Models;
using System.Reflection;
using AutocompleteDto = Dtos.AutocompleteDto;

public class AutocompleteMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Models.AutoComplete, AutocompleteDto>()
            .ConstructUsing(x => new AutoCompleteDto(x.Id, x.AutoCompleteNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.AutoCompleteDate, x.Status, x.Price));

        config.NewConfig<CreateAutoCompleteMongo, AutoCompleteSessionReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.AutoCompleteId, s => s.Id);

        config.NewConfig<Models.AutoComplete, AutoCompleteSessionReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.AutoCompleteId, s => s.Id.Value);

        config.NewConfig<AutoCompleteSessionReadModel, AutocompleteDto>()
            .Map(d => d.Id, s => s.AutoCompleteId);

        config.NewConfig<UpdateAutoCompleteMongo, AutoCompleteSessionReadModel>()
            .Map(d => d.AutoCompleteId, s => s.Id);

        config.NewConfig<AutocompleteMongo, AutoCompleteSessionReadModel>()
            .Map(d => d.AutoCompleteId, s => s.Id);

        config.NewConfig<CreateAutoCompleteRequestDto, CreateAutoComplete>()
            .ConstructUsing(x => new CreateAutoComplete(x.AutoCompleteNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate, x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.AutoCompleteDate, x.Status, x.Price));

        config.NewConfig<UpdateAutoCompleteRequestDto, UpdateAutoComplete>()
            .ConstructUsing(x => new UpdateAutoComplete(x.Id, x.AutoCompleteNumber, x.AircraftId, x.DepartureAirportId, x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.AutoCompleteDate, x.Status, x.IsDeleted, x.Price));

    }
}