using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Web;
using AiOrchestration.Extensions;
using FluentValidation;
using LearningAssistant.Data;
using LearningAssistant.Data.Seed;
using LearningAssistant.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace LearningAssistant.Extensions;


public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddAssistantModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<AssistantEventMapper>();
        builder.AddMinimalEndpoints(assemblies: typeof(AssistantRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(AssistantRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(AssistantRoot).Assembly);
        builder.AddCustomDbContext<LearningDbContext>(nameof(Assistant));
        builder.Services.AddScoped<IDataSeeder, ProfileDataSeeder>();

        builder.Services.AddCustomMediatR();

        // Register AI Orchestration
        builder.Services.AddAiOrchestration();

        // Register AI Chat Client
        // In a real scenario, this would be configured with a real provider (e.g. OpenAI, Llama)
        builder.Services.AddSingleton<IChatClient, SimulatedChatClient>();

        return builder;
    }


    public static WebApplication UseAssistantModules(this WebApplication app)
    { 
        app.UseMigration<LearningDbContext>();
        app.MapGrpcService<AssistantGrpcService>();

        return app;
    }
}