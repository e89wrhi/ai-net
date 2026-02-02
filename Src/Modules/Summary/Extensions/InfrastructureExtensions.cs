using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Web;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Summary.Data;
using Summary.Data.Seed;

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

        return builder;
    }


    public static WebApplication UseSummaryModules(this WebApplication app)
    { 
        app.UseMigration<SummaryDbContext>();
        app.MapGrpcService<SummaryGrpcService>();

        return app;
    }
}