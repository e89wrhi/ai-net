using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Web;
using AiOrchestration.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using SimplePlugin.Services;

namespace SimplePlugin.Extensions;


public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddSimplePluginModules(this WebApplicationBuilder builder)
    {
        builder.AddMinimalEndpoints(assemblies: typeof(SimplePluginRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(SimplePluginRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(SimplePluginRoot).Assembly);

        builder.Services.AddCustomMediatR();

        // Register AI Orchestration
        builder.Services.AddAiOrchestration();

        // Register AI Chat Client
        // In a real scenario, this would be configured with a real provider (e.g. OpenAI, Llama)
        builder.Services.AddSingleton<IChatClient, SimulatedChatClient>();

        return builder;
    }


    public static WebApplication UseSimplePluginModules(this WebApplication app)
    { 
        return app;
    }
}