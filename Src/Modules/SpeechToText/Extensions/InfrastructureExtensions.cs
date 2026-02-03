using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Web;
using AiOrchestration.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using SpeechToText.Data;
using SpeechToText.Data.Seed;
using SpeechToText.Services;

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

        // Register AI Orchestration
        builder.Services.AddAiOrchestration();

        // Register AI Chat Client
        // In a real scenario, this would be configured with a real provider (e.g. OpenAI, Llama)
        builder.Services.AddSingleton<IChatClient, SimulatedChatClient>();

        return builder;
    }


    public static WebApplication UseSpeechToTextModules(this WebApplication app)
    { 
        app.UseMigration<SpeechToTextDbContext>();
        app.MapGrpcService<SpeechToTextGrpcService>();

        return app;
    }
}