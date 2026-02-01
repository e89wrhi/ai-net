using Mapster;
using System.Reflection;

namespace ChatBot.Features;

using Features.DeleteChat.V1;
using Features.SendMessage.V1;
using Features.GetChatHistory.V1;
using Features.StartChat.V1;
using MassTransit;
using Models;
using System.Reflection;
using ChatDto = Dtos.ChatDto;

public class ChatMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Models.Chat, ChatDto>()
            .ConstructUsing(x => new ChatDto(x.Id, x.ChatNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.ChatDate, x.Status, x.Price));

        config.NewConfig<CreateChatMongo, ChatSessionReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.ChatId, s => s.Id);

        config.NewConfig<Models.Chat, ChatSessionReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.ChatId, s => s.Id.Value);

        config.NewConfig<ChatSessionReadModel, ChatDto>()
            .Map(d => d.Id, s => s.ChatId);

        config.NewConfig<UpdateChatMongo, ChatSessionReadModel>()
            .Map(d => d.ChatId, s => s.Id);

        config.NewConfig<DeleteChatMongo, ChatSessionReadModel>()
            .Map(d => d.ChatId, s => s.Id);

        config.NewConfig<CreateChatRequestDto, CreateChat>()
            .ConstructUsing(x => new CreateChat(x.ChatNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate, x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.ChatDate, x.Status, x.Price));

        config.NewConfig<UpdateChatRequestDto, UpdateChat>()
            .ConstructUsing(x => new UpdateChat(x.Id, x.ChatNumber, x.AircraftId, x.DepartureAirportId, x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.ChatDate, x.Status, x.IsDeleted, x.Price));

    }
}