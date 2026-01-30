using Mapster;
using System.Reflection;

namespace LearningAssistant.Features;

using Features.GenerateLesson.V1;
using Features.GenerateQuiz.V1;
using MassTransit;
using Models;
using System.Reflection;
using Features.SubmitQuiz.V1;
using AssistantDto = Dtos.AssistantDto;

public class AssistantMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Models.Assistant, AssistantDto>()
            .ConstructUsing(x => new AssistantDto(x.Id, x.AssistantNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.AssistantDate, x.Status, x.Price));

        config.NewConfig<CreateAssistantMongo, AssistantReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.AssistantId, s => s.Id);

        config.NewConfig<Models.Assistant, AssistantReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.AssistantId, s => s.Id.Value);

        config.NewConfig<AssistantReadModel, AssistantDto>()
            .Map(d => d.Id, s => s.AssistantId);

        config.NewConfig<UpdateAssistantMongo, AssistantReadModel>()
            .Map(d => d.AssistantId, s => s.Id);

        config.NewConfig<DeleteAssistantMongo, AssistantReadModel>()
            .Map(d => d.AssistantId, s => s.Id);

        config.NewConfig<CreateAssistantRequestDto, CreateAssistant>()
            .ConstructUsing(x => new CreateAssistant(x.AssistantNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate, x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.AssistantDate, x.Status, x.Price));

        config.NewConfig<UpdateAssistantRequestDto, UpdateAssistant>()
            .ConstructUsing(x => new UpdateAssistant(x.Id, x.AssistantNumber, x.AircraftId, x.DepartureAirportId, x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.AssistantDate, x.Status, x.IsDeleted, x.Price));

    }
}