using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Web;
using AiOrchestration.Extensions;
using FluentValidation;
using ImageCaption.Data;
using ImageCaption.Data.Seed;
using ImageCaption.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace ImageCaption.Extensions;


public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddImageModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ImageEventMapper>();
        builder.AddMinimalEndpoints(assemblies: typeof(ImageRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(ImageRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(ImageRoot).Assembly);
        builder.AddCustomDbContext<ImageCaptionDbContext>(nameof(Image));
        builder.Services.AddScoped<IDataSeeder, ImageDataSeeder>();

        builder.Services.AddCustomMediatR();

        // Register AI Orchestration
        builder.Services.AddAiOrchestration();

        // Register AI Chat Client
        // In a real scenario, this would be configured with a real provider (e.g. OpenAI, Llama)
        builder.Services.AddSingleton<IChatClient, SimulatedChatClient>();

        return builder;
    }


    public static WebApplication UseImageModules(this WebApplication app)
    { 
        app.UseMigration<ImageCaptionDbContext>();
        app.MapGrpcService<ImageGrpcService>();

        return app;
    }
}