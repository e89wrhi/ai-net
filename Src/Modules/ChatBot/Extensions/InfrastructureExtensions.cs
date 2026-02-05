using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Web;
using AiOrchestration.Extensions;
using ChatBot.Data;
using ChatBot.Data.Seed;
using ChatBot.Services;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace ChatBot.Extensions;


public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddChatModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ChatEventMapper>();
        builder.AddMinimalEndpoints(assemblies: typeof(ChatRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(ChatRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(ChatRoot).Assembly);
        builder.AddCustomDbContext<ChatDbContext>(nameof(ChatBot));
        builder.Services.AddScoped<IDataSeeder, ChatDataSeeder>();

        builder.Services.AddCustomMediatR();

        // Register AI Orchestration
        builder.Services.AddAiOrchestration();

        // Register AI Chat Client
        // In a real scenario, this would be configured with a real provider (e.g. OpenAI, Llama)
        builder.Services.AddSingleton<IChatClient, SimulatedChatClient>();

        return builder;
    }


    public static WebApplication UseChatModules(this WebApplication app)
    { 
        app.UseMigration<ChatDbContext>();
        // app.MapGrpcService<ChatGrpcService>();

        return app;
    }
}