using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Web;
using AiOrchestration.Extensions;
using FluentValidation;
using ImageEdit.Data;
using ImageEdit.Data.Seed;
using ImageEdit.GrpcServer.Services;
using ImageEdit.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace ImageEdit.Extensions;


public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddImageEditModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ImageEditEventMapper>();
        builder.AddMinimalEndpoints(assemblies: typeof(ImageEditRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(ImageEditRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(ImageEditRoot).Assembly);
        builder.AddCustomDbContext<ImageEditDbContext>(nameof(ImageEdit));
        builder.Services.AddScoped<IDataSeeder, ImageEditDataSeeder>();

        builder.Services.AddCustomMediatR();

        // Register AI Orchestration
        builder.Services.AddAiOrchestration();

        // Register AI Chat Client
        // In a real scenario, this would be configured with a real provider (e.g. OpenAI, Llama)
        builder.Services.AddSingleton<IChatClient, SimulatedChatClient>();

        return builder;
    }


    public static WebApplication UseImageEditModules(this WebApplication app)
    { 
        app.UseMigration<ImageEditDbContext>();
        app.MapGrpcService<ImageEditGrpcService>();

        return app;
    }
}