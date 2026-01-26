using EventStore.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AI.Common.Web;
using System.Reflection;

namespace AI.Common.EventStoreDB;

/// <summary>
/// Root configuration for EventStoreDB, typically provided via appsettings.json.
/// </summary>
public class EventStoreOptions
{
    public string ConnectionString { get; set; } = default!;
}


public record EventStoreDBOptions(
    bool UseInternalCheckpointing = true
);

/// <summary>
/// Dependency injection setup for EventStoreDB.
/// It configures the connection, repositories, subscriptions, and projections.
/// </summary>
public static class EventStoreDBConfigExtensions
{
    public static IServiceCollection AddEventStoreDB(this IServiceCollection services, IConfiguration configuration,
        EventStoreDBOptions? options = null)
    {

        services
            .AddSingleton(x =>
            {
                var aspireConnectionString = configuration.GetConnectionString("eventstore");
                var eventStoreOptions = services.GetOptions<EventStoreOptions>(nameof(EventStoreOptions));
                return new EventStoreClient(EventStoreClientSettings.Create(aspireConnectionString ?? eventStoreOptions.ConnectionString));
            })
            .AddScoped(typeof(IEventStoreDBRepository<>), typeof(EventStoreDBRepository<>))
            .AddTransient<EventStoreDBSubscriptionToAll, EventStoreDBSubscriptionToAll>();

        if (options?.UseInternalCheckpointing != false)
            services.AddTransient<ISubscriptionCheckpointRepository, EventStoreDBSubscriptionCheckpointRepository>();

        return services;
    }

    public static IServiceCollection AddEventStoreDBSubscriptionToAll(
        this IServiceCollection services,
        EventStoreDBSubscriptionToAllOptions? subscriptionOptions = null,
        bool checkpointToEventStoreDB = true)
    {
        if (checkpointToEventStoreDB)
            services.AddTransient<ISubscriptionCheckpointRepository, EventStoreDBSubscriptionCheckpointRepository>();

        return services.AddHostedService(serviceProvider =>
            {
                var logger =
                    serviceProvider.GetRequiredService<ILogger<BackgroundWorker>>();

                var eventStoreDBSubscriptionToAll =
                    serviceProvider.GetRequiredService<EventStoreDBSubscriptionToAll>();

                return new BackgroundWorker(
                    logger,
                    ct =>
                        eventStoreDBSubscriptionToAll.SubscribeToAll(
                            subscriptionOptions ?? new EventStoreDBSubscriptionToAllOptions(),
                            ct
                        )
                );
            }
        );
    }

    public static IServiceCollection AddProjections(this IServiceCollection services,
        params Assembly[] assembliesToScan)
    {
        services.AddSingleton<IProjectionPublisher, ProjectionPublisher>();

        RegisterProjections(services, assembliesToScan!);

        return services;
    }

    private static void RegisterProjections(IServiceCollection services, Assembly[] assembliesToScan)
    {
        services.Scan(scan => scan
            .FromAssemblies(assembliesToScan)
            .AddClasses(classes => classes.AssignableTo<IProjectionProcessor>()) // Filter classes
            .AsImplementedInterfaces()
            .WithTransientLifetime());
    }
}