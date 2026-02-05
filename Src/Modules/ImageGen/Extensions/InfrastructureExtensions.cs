using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Web;
using AiOrchestration.Extensions;
using FluentValidation;
using ImageGen.Data;
using ImageGen.Data.Seed;
using ImageGen.GrpcServer.Services;
using ImageGen.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace ImageGen.Extensions;


public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddImageGenModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ImageGenEventMapper>();
        builder.AddMinimalEndpoints(assemblies: typeof(ImageGenRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(ImageGenRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(ImageGenRoot).Assembly);
        builder.AddCustomDbContext<ImageGenDbContext>(nameof(ImageGen));
        builder.Services.AddScoped<IDataSeeder, ImageGenDataSeeder>();

        builder.Services.AddCustomMediatR();

        // Register AI Orchestration
        builder.Services.AddAiOrchestration();

        // Register AI Chat Client
        // In a real scenario, this would be configured with a real provider (e.g. OpenAI, Llama)
        builder.Services.AddSingleton<IChatClient, SimulatedChatClient>();

        return builder;
    }


    public static WebApplication UseImageGenModules(this WebApplication app)
    { 
        app.UseMigration<ImageGenDbContext>();
        app.MapGrpcService<ImageGenGrpcService>();

        return app;
    }
}