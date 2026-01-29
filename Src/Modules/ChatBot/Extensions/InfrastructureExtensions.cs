using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Mongo;
using AI.Common.Web;
using ChatBot;
using ChatBot.Data;
using ChatBot.Data.Seed;
using ChatBot.Extensions;
using ChatBot.GrpcServer.Services;
using FluentValidation;
using MassTransit.Contracts;
using Microsoft.AspNetCore.Builder;
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
        builder.AddMongoDbContext<ChatReadDbContext>();

        builder.Services.AddCustomMediatR();

        return builder;
    }


    public static WebApplication UseChatModules(this WebApplication app)
    { 
        app.UseMigration<ChatDbContext>();
        app.MapGrpcService<ChatGrpcServices>();

        return app;
    }
}