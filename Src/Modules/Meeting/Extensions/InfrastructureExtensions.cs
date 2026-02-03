using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Web;
using AiOrchestration.Extensions;
using FluentValidation;
using Meeting.Data;
using Meeting.Data.Seed;
using Meeting.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace Meeting.Extensions;


public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddMeetingModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<MeetingEventMapper>();
        builder.AddMinimalEndpoints(assemblies: typeof(MeetingRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(MeetingRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(MeetingRoot).Assembly);
        builder.AddCustomDbContext<MeetingDbContext>(nameof(Meeting));
        builder.Services.AddScoped<IDataSeeder, MeetingDataSeeder>();

        builder.Services.AddCustomMediatR();

        // Register AI Orchestration
        builder.Services.AddAiOrchestration();

        // Register AI Chat Client
        // In a real scenario, this would be configured with a real provider (e.g. OpenAI, Llama)
        builder.Services.AddSingleton<IChatClient, SimulatedChatClient>();

        return builder;
    }


    public static WebApplication UseMeetingModules(this WebApplication app)
    { 
        app.UseMigration<MeetingDbContext>();
        app.MapGrpcService<MeetingGrpcService>();

        return app;
    }
}