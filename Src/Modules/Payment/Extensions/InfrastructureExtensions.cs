using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Mongo;
using AI.Common.Web;
using Payment;
using Payment.Data;
using Payment.Data.Seed;
using Payment.Extensions;
using Payment.GrpcServer.Services;
using FluentValidation;
using MassTransit.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Payment.Extensions;


public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddPaymentModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<PaymentEventMapper>();
        builder.AddMinimalEndpoints(assemblies: typeof(PaymentRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(PaymentRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(PaymentRoot).Assembly);
        builder.AddCustomDbContext<PaymentDbContext>(nameof(Payment));
        builder.Services.AddScoped<IDataSeeder, PaymentDataSeeder>();
        builder.AddMongoDbContext<PaymentReadDbContext>();

        builder.Services.AddCustomMediatR();

        return builder;
    }


    public static WebApplication UsePaymentModules(this WebApplication app)
    { 
        app.UseMigration<PaymentDbContext>();
        app.MapGrpcService<PaymentGrpcServices>();

        return app;
    }
}