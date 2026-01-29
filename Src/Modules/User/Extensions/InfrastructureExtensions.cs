using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Mongo;
using AI.Common.Web;
using User;
using User.Data;
using User.Data.Seed;
using User.Extensions;
using User.GrpcServer.Services;
using FluentValidation;
using MassTransit.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace User.Extensions;


public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddUserModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<UserEventMapper>();
        builder.AddMinimalEndpoints(assemblies: typeof(UserRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(UserRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(UserRoot).Assembly);
        builder.AddCustomDbContext<UserDbContext>(nameof(User));
        builder.Services.AddScoped<IDataSeeder, UserDataSeeder>();
        builder.AddMongoDbContext<UserReadDbContext>();

        builder.Services.AddCustomMediatR();

        return builder;
    }


    public static WebApplication UseUserModules(this WebApplication app)
    { 
        app.UseMigration<UserDbContext>();
        app.MapGrpcService<UserGrpcServices>();

        return app;
    }
}