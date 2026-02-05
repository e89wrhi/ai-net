using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Web;
using AiOrchestration.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Sentiment.Data;
using Sentiment.Data.Seed;
using Sentiment.GrpcServer.Services;
using Sentiment.Services;

namespace Sentiment.Extensions;


public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddSentimentModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<SentimentEventMapper>();
        builder.AddMinimalEndpoints(assemblies: typeof(SentimentRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(SentimentRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(SentimentRoot).Assembly);
        builder.AddCustomDbContext<SentimentDbContext>(nameof(Sentiment));
        builder.Services.AddScoped<IDataSeeder, SentimentDataSeeder>();

        builder.Services.AddCustomMediatR();

        // Register AI Orchestration
        builder.Services.AddAiOrchestration();

        // Register AI Chat Client
        // In a real scenario, this would be configured with a real provider (e.g. OpenAI, Llama)
        builder.Services.AddSingleton<IChatClient, SimulatedChatClient>();

        return builder;
    }


    public static WebApplication UseSentimentModules(this WebApplication app)
    { 
        app.UseMigration<SentimentDbContext>();
        app.MapGrpcService<SentimentGrpcService>();

        return app;
    }
}