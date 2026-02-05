using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Web;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using User.Data;
using User.Data.Seed;
using User.GrpcServer.Services;

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

        builder.Services.AddCustomMediatR();

        return builder;
    }


    public static WebApplication UseUserModules(this WebApplication app)
    { 
        app.UseMigration<UserDbContext>();
        app.MapGrpcService<UserGrpcService>();

        return app;
    }
}