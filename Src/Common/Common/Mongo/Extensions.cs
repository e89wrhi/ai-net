
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AI.Common.Web;

namespace AI.Common.Mongo;

/// <summary>
/// Extension methods for setting up MongoDB in the application.
/// It simplifies the registration of MongoDbContext, repositories, and unit of work.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Registers a MongoDbContext for the given service collection.
    /// It handles configuration, Aspire-aware connection strings, and scoping.
    /// </summary>
    public static IServiceCollection AddMongoDbContext<TContext>(
        this WebApplicationBuilder builder, Action<MongoOptions>? configurator = null)
    where TContext : MongoDbContext
    {
        return builder.Services.AddMongoDbContext<TContext, TContext>(builder.Configuration, configurator);
    }

    /// <summary>
    /// Registers a MongoDbContext with its service and implementation types separately.
    /// </summary>
    public static IServiceCollection AddMongoDbContext<TContextService, TContextImplementation>(
        this IServiceCollection services, IConfiguration configuration, Action<MongoOptions>? configurator = null)
    where TContextService : IMongoDbContext
    where TContextImplementation : MongoDbContext, TContextService
    {
        // Configure MongoOptions with Aspire-aware defaults
        services.AddOptions<MongoOptions>()
            .Bind(configuration.GetSection(nameof(MongoOptions)))
            .PostConfigure(options =>
                           {
                               var aspireConnectionString = configuration.GetConnectionString("mongo");
                               options.ConnectionString = aspireConnectionString ?? options.ConnectionString;
                           });

        if (configurator is { })
        {
            services.Configure(nameof(MongoOptions), configurator);
        }
        else
        {
            services.AddValidateOptions<MongoOptions>();
        }

        // Register the context and its interfaces
        services.AddScoped(typeof(TContextService), typeof(TContextImplementation));
        services.AddScoped(typeof(TContextImplementation));

        services.AddScoped<IMongoDbContext>(sp => sp.GetRequiredService<TContextService>());

        // Register generic Mongo repository and unit of work
        services.AddTransient(typeof(IMongoRepository<,>), typeof(MongoRepository<,>));
        services.AddTransient(typeof(IMongoUnitOfWork<>), typeof(MongoUnitOfWork<>));

        return services;
    }
}