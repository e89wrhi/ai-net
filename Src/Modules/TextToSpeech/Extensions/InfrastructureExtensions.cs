using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Web;
using AiOrchestration.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using TextToSpeech.Data;
using TextToSpeech.Data.Seed;
using TextToSpeech.GrpcServer.Services;
using TextToSpeech.Services;

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

        builder.Services.AddCustomMediatR();

        // Register AI Orchestration
        builder.Services.AddAiOrchestration();

        // Register AI Chat Client
        // In a real scenario, this would be configured with a real provider (e.g. OpenAI, Llama)
        builder.Services.AddSingleton<IChatClient, SimulatedChatClient>();

        return builder;
    }


    public static WebApplication UseTextToSpeechModules(this WebApplication app)
    { 
        app.UseMigration<TextToSpeechDbContext>();
        app.MapGrpcService<TextToSpeechGrpcService>();

        return app;
    }
}