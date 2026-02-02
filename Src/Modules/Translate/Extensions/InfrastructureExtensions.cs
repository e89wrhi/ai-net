using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Web;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Translate.Data;
using Translate.Data.Seed;

namespace Translate.Extensions;


public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddTranslateModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<TranslateEventMapper>();
        builder.AddMinimalEndpoints(assemblies: typeof(TranslateRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(TranslateRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(TranslateRoot).Assembly);
        builder.AddCustomDbContext<TranslateDbContext>(nameof(Translate));
        builder.Services.AddScoped<IDataSeeder, TranslateDataSeeder>();

        builder.Services.AddCustomMediatR();

        return builder;
    }


    public static WebApplication UseTranslateModules(this WebApplication app)
    { 
        app.UseMigration<TranslateDbContext>();
        app.MapGrpcService<TranslateGrpcService>();

        return app;
    }
}