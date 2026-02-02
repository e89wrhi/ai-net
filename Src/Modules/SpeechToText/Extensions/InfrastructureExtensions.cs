using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Web;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SpeechToText.Data;
using SpeechToText.Data.Seed;

namespace SpeechToText.Extensions;


public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddSpeechToTextModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<SpeechToTextEventMapper>();
        builder.AddMinimalEndpoints(assemblies: typeof(SpeechToTextRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(SpeechToTextRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(SpeechToTextRoot).Assembly);
        builder.AddCustomDbContext<SpeechToTextDbContext>(nameof(SpeechToText));
        builder.Services.AddScoped<IDataSeeder, SpeechToTextDataSeeder>();

        builder.Services.AddCustomMediatR();

        return builder;
    }


    public static WebApplication UseSpeechToTextModules(this WebApplication app)
    { 
        app.UseMigration<SpeechToTextDbContext>();
        app.MapGrpcService<SpeechToTextGrpcService>();

        return app;
    }
}