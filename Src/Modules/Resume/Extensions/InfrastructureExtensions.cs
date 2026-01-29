using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Mongo;
using AI.Common.Web;
using Resume;
using Resume.Data;
using Resume.Data.Seed;
using Resume.Extensions;
using Resume.GrpcServer.Services;
using FluentValidation;
using MassTransit.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Resume.Extensions;


public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddResumeModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ResumeEventMapper>();
        builder.AddMinimalEndpoints(assemblies: typeof(ResumeRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(ResumeRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(ResumeRoot).Assembly);
        builder.AddCustomDbContext<ResumeDbContext>(nameof(Resume));
        builder.Services.AddScoped<IDataSeeder, ResumeDataSeeder>();
        builder.AddMongoDbContext<ResumeReadDbContext>();

        builder.Services.AddCustomMediatR();

        return builder;
    }


    public static WebApplication UseResumeModules(this WebApplication app)
    { 
        app.UseMigration<ResumeDbContext>();
        app.MapGrpcService<ResumeGrpcServices>();

        return app;
    }
}