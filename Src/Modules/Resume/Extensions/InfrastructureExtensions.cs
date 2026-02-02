using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Web;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Resume.Data;
using Resume.Data.Seed;

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

        builder.Services.AddCustomMediatR();

        return builder;
    }


    public static WebApplication UseResumeModules(this WebApplication app)
    { 
        app.UseMigration<ResumeDbContext>();
        app.MapGrpcService<ResumeGrpcService>();

        return app;
    }
}