using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Web;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Payment.Data;
using Payment.Data.Seed;
using Payment.GrpcServer.Services;

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

        builder.Services.AddCustomMediatR();

        return builder;
    }


    public static WebApplication UsePaymentModules(this WebApplication app)
    { 
        app.UseMigration<PaymentDbContext>();
        app.MapGrpcService<PaymentGrpcService>();

        return app;
    }
}