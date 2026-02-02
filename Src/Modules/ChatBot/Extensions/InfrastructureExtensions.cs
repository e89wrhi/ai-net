using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Web;
using ChatBot.Data;
using ChatBot.Data.Seed;
using FluentValidation;
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

        builder.Services.AddCustomMediatR();

        return builder;
    }


    public static WebApplication UseChatModules(this WebApplication app)
    { 
        app.UseMigration<ChatDbContext>();
        app.MapGrpcService<ChatGrpcService>();

        return app;
    }
}