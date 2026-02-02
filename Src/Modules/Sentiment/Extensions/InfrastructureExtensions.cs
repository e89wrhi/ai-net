using AI.Common.EFCore;
using AI.Common.Mapster;
using AI.Common.Web;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Sentiment.Data;
using Sentiment.Data.Seed;

namespace Sentiment.Extensions;


public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddSentimentModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<SentimentEventMapper>();
        builder.AddMinimalEndpoints(assemblies: typeof(SentimentRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(SentimentRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(SentimentRoot).Assembly);
        builder.AddCustomDbContext<SentimentDbContext>(nameof(Sentiment));
        builder.Services.AddScoped<IDataSeeder, SentimentDataSeeder>();

        builder.Services.AddCustomMediatR();

        return builder;
    }


    public static WebApplication UseSentimentModules(this WebApplication app)
    { 
        app.UseMigration<SentimentDbContext>();
        app.MapGrpcService<SentimentGrpcService>();

        return app;
    }
}