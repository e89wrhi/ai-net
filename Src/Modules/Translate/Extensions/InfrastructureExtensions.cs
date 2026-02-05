using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Web;
using AiOrchestration.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Translate.Data;
using Translate.Data.Seed;
using Translate.GrpcServer.Services;
using Translate.Services;

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

        // Register AI Orchestration
        builder.Services.AddAiOrchestration();

        // Register AI Chat Client
        // In a real scenario, this would be configured with a real provider (e.g. OpenAI, Llama)
        builder.Services.AddSingleton<IChatClient, SimulatedChatClient>();

        return builder;
    }


    public static WebApplication UseTranslateModules(this WebApplication app)
    { 
        app.UseMigration<TranslateDbContext>();
        app.MapGrpcService<TranslateGrpcService>();

        return app;
    }
}