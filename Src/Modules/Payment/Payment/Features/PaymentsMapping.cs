using Mapster;
using System.Reflection;

namespace Payment.Features;

using Features.CancelSubscription.V1;
using Features.CreateSubscription.V1;
using MassTransit;
using Models;
using System.Reflection;
using Features.GenerateInvoice.V1;
using Features.GetInvoices.V1;
using Features.GetSubscription.V1;
using Features.RecordUsageCharge.V1;
using PaymentDto = Dtos.SubscriptionDto;

public class PaymentMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Models.SubscriptionModel, PaymentDto>()
            .Map(d => d.Id, s => s.Id.Value.ToString())
            .Map(d => d.HomeTeam, s => s.HomeTeam)
            .Map(d => d.AwayTeam, s => s.AwayTeam)
            .Map(d => d.HomeTeamScore, s => s.HomeTeamScore)
            .Map(d => d.AwayTeamScore, s => s.AwayTeamScore)
            .Map(d => d.League, s => s.League)
            .Map(d => d.Status, s => s.Status)
            .Map(d => d.MatchTime, s => s.MatchTime)
            .Map(d => d.EventsCount, s => s.EventsCount)
            .Map(d => d.HomeVotesCount, s => s.HomeVotesCount)
            .Map(d => d.AwayVotesCount, s => s.AwayVotesCount)
            .Map(d => d.DrawVotesCount, s => s.DrawVotesCount);

        config.NewConfig<CreatePaymentMongo, PaymentReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.PaymentId, s => s.Id);

        config.NewConfig<Models.Payment, PaymentReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.PaymentId, s => s.Id.Value);

        config.NewConfig<PaymentReadModel, PaymentDto>()
            .Map(d => d.Id, s => s.PaymentId);

        config.NewConfig<UpdatePaymentMongo, PaymentReadModel>()
            .Map(d => d.PaymentId, s => s.Id);

        config.NewConfig<DeletePaymentMongo, PaymentReadModel>()
            .Map(d => d.PaymentId, s => s.Id);

        config.NewConfig<CreatePaymentRequestDto, CreatePayment>()
            .ConstructUsing(x => new CreatePayment(x.PaymentNumber, x.AircraftId, x.DepartureAirportId,
                x.DepartureDate, x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.PaymentDate, x.Status, x.Price));

        config.NewConfig<UpdatePaymentRequestDto, UpdatePayment>()
            .ConstructUsing(x => new UpdatePayment(x.Id, x.PaymentNumber, x.AircraftId, x.DepartureAirportId, x.DepartureDate,
                x.ArriveDate, x.ArriveAirportId, x.DurationMinutes, x.PaymentDate, x.Status, x.IsDeleted, x.Price));

    }
}