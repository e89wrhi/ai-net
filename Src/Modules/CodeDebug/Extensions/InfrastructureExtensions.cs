using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Web;
using AiOrchestration.Extensions;
using CodeDebug.Data;
using CodeDebug.Data.Seed;
using CodeDebug.GrpcServer.Services;
using CodeDebug.Services;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace CodeDebug.Extensions;


public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddCodeDebugModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<CodeDebugEventMapper>();
        builder.AddMinimalEndpoints(assemblies: typeof(CodeDebugRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(CodeDebugRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(CodeDebugRoot).Assembly);
        builder.AddCustomDbContext<CodeDebugDbContext>(nameof(CodeDebug));
        builder.Services.AddScoped<IDataSeeder, CodeDebugDataSeeder>();

        builder.Services.AddCustomMediatR();

        // Register AI Orchestration
        builder.Services.AddAiOrchestration();

        // Register AI Chat Client
        // In a real scenario, this would be configured with a real provider (e.g. OpenAI, Llama)
        builder.Services.AddSingleton<IChatClient, SimulatedChatClient>();

        return builder;
    }


    public static WebApplication UseCodeDebugModules(this WebApplication app)
    { 
        app.UseMigration<CodeDebugDbContext>();
        app.MapGrpcService<CodeDebugGrpcService>();

        return app;
    }
}