using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Web;
using AiOrchestration.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Summary.Data;
using Summary.Data.Seed;
using Summary.GrpcServer.Services;
using Summary.Services;

namespace Summary.Extensions;


public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddSummaryModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<SummaryEventMapper>();
        builder.AddMinimalEndpoints(assemblies: typeof(SummaryRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(SummaryRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(SummaryRoot).Assembly);
        builder.AddCustomDbContext<SummaryDbContext>(nameof(Summary));
        builder.Services.AddScoped<IDataSeeder, SummaryDataSeeder>();

        builder.Services.AddCustomMediatR();

        // Register AI Orchestration
        builder.Services.AddAiOrchestration();

        // Register AI Chat Client
        // In a real scenario, this would be configured with a real provider (e.g. OpenAI, Llama)
        builder.Services.AddSingleton<IChatClient, SimulatedChatClient>();

        return builder;
    }


    public static WebApplication UseSummaryModules(this WebApplication app)
    { 
        app.UseMigration<SummaryDbContext>();
        app.MapGrpcService<SummaryGrpcService>();

        return app;
    }
}