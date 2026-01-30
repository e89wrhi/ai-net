using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Mongo;
using AI.Common.Web;
using LearningAssistant;
using LearningAssistant.Data;
using LearningAssistant.Data.Seed;
using LearningAssistant.Extensions;
using LearningAssistant.GrpcServer.Services;
using FluentValidation;
using MassTransit.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace LearningAssistant.Extensions;


public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddAssistantModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<AssistantEventMapper>();
        builder.AddMinimalEndpoints(assemblies: typeof(AssistantRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(AssistantRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(AssistantRoot).Assembly);
        builder.AddCustomDbContext<AssistantDbContext>(nameof(Assistant));
        builder.Services.AddScoped<IDataSeeder, ProfileDataSeeder>();
        builder.AddMongoDbContext<ProfileReadDbContext>();

        builder.Services.AddCustomMediatR();

        return builder;
    }


    public static WebApplication UseAssistantModules(this WebApplication app)
    { 
        app.UseMigration<AssistantDbContext>();
        app.MapGrpcService<AssistantGrpcService>();

        return app;
    }
}