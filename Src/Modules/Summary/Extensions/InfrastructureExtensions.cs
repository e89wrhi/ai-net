using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Mongo;
using AI.Common.Web;
using Summary;
using Summary.Data;
using Summary.Data.Seed;
using Summary.Extensions;
using Summary.GrpcServer.Services;
using FluentValidation;
using MassTransit.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Grpc.AspNetCore;

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
        builder.AddMongoDbContext<SummaryReadDbContext>();

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