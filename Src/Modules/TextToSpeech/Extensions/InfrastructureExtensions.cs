using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Mongo;
using AI.Common.Web;
using TextToSpeech;
using TextToSpeech.Data;
using TextToSpeech.Data.Seed;
using TextToSpeech.Extensions;
using TextToSpeech.GrpcServer.Services;
using FluentValidation;
using MassTransit.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Grpc.AspNetCore;

namespace TextToSpeech.Extensions;


public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddTextToSpeechModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<TextToSpeechEventMapper>();
        builder.AddMinimalEndpoints(assemblies: typeof(TextToSpeechRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(TextToSpeechRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(TextToSpeechRoot).Assembly);
        builder.AddCustomDbContext<TextToSpeechDbContext>(nameof(TextToSpeech));
        builder.Services.AddScoped<IDataSeeder, TextToSpeechDataSeeder>();
        builder.AddMongoDbContext<TextToSpeechReadDbContext>();

        builder.Services.AddCustomMediatR();

        return builder;
    }


    public static WebApplication UseTextToSpeechModules(this WebApplication app)
    { 
        app.UseMigration<TextToSpeechDbContext>();
        app.MapGrpcService<TextToSpeechGrpcService>();

        return app;
    }
}