using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Mongo;
using AI.Common.Web;
using ImageEdit;
using ImageEdit.Data;
using ImageEdit.Data.Seed;
using ImageEdit.Extensions;
using ImageEdit.GrpcServer.Services;
using FluentValidation;
using MassTransit.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Grpc.AspNetCore;

namespace ImageEdit.Extensions;


public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddImageEditModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ImageEditEventMapper>();
        builder.AddMinimalEndpoints(assemblies: typeof(ImageEditRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(ImageEditRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(ImageEditRoot).Assembly);
        builder.AddCustomDbContext<ImageEditDbContext>(nameof(ImageEdit));
        builder.Services.AddScoped<IDataSeeder, ImageEditDataSeeder>();
        builder.AddMongoDbContext<ImageEditReadDbContext>();

        builder.Services.AddCustomMediatR();

        return builder;
    }


    public static WebApplication UseImageEditModules(this WebApplication app)
    { 
        app.UseMigration<ImageEditDbContext>();
        app.MapGrpcService<ImageEditGrpcService>();

        return app;
    }
}