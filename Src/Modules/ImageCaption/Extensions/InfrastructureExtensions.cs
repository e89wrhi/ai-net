using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Mongo;
using AI.Common.Web;
using ImageCaption;
using ImageCaption.Data;
using ImageCaption.Data.Seed;
using ImageCaption.Extensions;
using ImageCaption.GrpcServer.Services;
using FluentValidation;
using MassTransit.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ImageCaption.Extensions;


public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddImageModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ImageEventMapper>();
        builder.AddMinimalEndpoints(assemblies: typeof(ImageRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(ImageRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(ImageRoot).Assembly);
        builder.AddCustomDbContext<ImageCaptionDbContext>(nameof(Image));
        builder.Services.AddScoped<IDataSeeder, ImageDataSeeder>();
        builder.AddMongoDbContext<ImageCaptionReadDbContext>();

        builder.Services.AddCustomMediatR();

        return builder;
    }


    public static WebApplication UseImageModules(this WebApplication app)
    { 
        app.UseMigration<ImageCaptionDbContext>();
        app.MapGrpcService<ImageGrpcService>();

        return app;
    }
}