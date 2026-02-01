using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Mongo;
using AI.Common.Web;
using ImageGen;
using ImageGen.Data;
using ImageGen.Data.Seed;
using ImageGen.Extensions;
using ImageGen.GrpcServer.Services;
using FluentValidation;
using MassTransit.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Grpc.AspNetCore;

namespace ImageGen.Extensions;


public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddImageGenModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ImageGenEventMapper>();
        builder.AddMinimalEndpoints(assemblies: typeof(ImageGenRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(ImageGenRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(ImageGenRoot).Assembly);
        builder.AddCustomDbContext<ImageGenDbContext>(nameof(ImageGen));
        builder.Services.AddScoped<IDataSeeder, ImageGenDataSeeder>();
        builder.AddMongoDbContext<ImageGenReadDbContext>();

        builder.Services.AddCustomMediatR();

        return builder;
    }


    public static WebApplication UseImageGenModules(this WebApplication app)
    { 
        app.UseMigration<ImageGenDbContext>();
        app.MapGrpcService<ImageGenGrpcService>();

        return app;
    }
}