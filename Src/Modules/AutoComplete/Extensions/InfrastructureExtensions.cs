using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Web;
using AutoComplete;
using AutoComplete.Data;
using AutoComplete.Data.Seed;
using AutoComplete.Extensions;
using AutoComplete.GrpcServer.Services;
using FluentValidation;
using MassTransit.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Grpc.AspNetCore;
using Microsoft.Extensions.AI;
using AutoComplete.Services;
using AiOrchestration.Extensions;


namespace AutoComplete.Extensions;


public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddAutoCompleteModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<AutoCompleteEventMapper>();
        builder.AddMinimalEndpoints(assemblies: typeof(AutoCompleteRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(AutoCompleteRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(AutoCompleteRoot).Assembly);
        builder.AddCustomDbContext<AutocompleteDbContext>(nameof(AutoComplete));
        builder.Services.AddScoped<IDataSeeder, AutocompleteDataSeeder>();
        // MongoDB removed - was redundant with SQL storage

        builder.Services.AddCustomMediatR();

        // Register AI Orchestration
        builder.Services.AddAiOrchestration();

        // Register AI Chat Client
        // In a real scenario, this would be configured with a real provider (e.g. OpenAI, Llama)
        builder.Services.AddSingleton<IChatClient, SimulatedChatClient>();


        return builder;
    }


    public static WebApplication UseAutoCompleteModules(this WebApplication app)
    { 
        app.UseMigration<AutocompleteDbContext>();
        app.MapGrpcService<AutoCompleteGrpcService>();

        return app;
    }
}