using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AI.Common.Web;
using System.Reflection;

namespace AI.Common.EventStoreDB;

/// <summary>
/// Infrastructure extensions for EventStoreDB.
/// It registers the event store client and sets up the projections that listen to the event stream.
/// </summary>
public static class Extensions
{
    // ref: https://github.com/oskardudycz/EventSourcing.NetCore/tree/main/Sample/EventStoreDB/ECommerce
    public static IServiceCollection AddEventStore(
        this IServiceCollection services,
        IConfiguration configuration,
        params Assembly[] assemblies
    )
    {
        services.AddValidateOptions<EventStoreOptions>();

        var assembliesToScan = assemblies.Length > 0 ? assemblies : new[] { Assembly.GetEntryAssembly()! };

        return services
            .AddEventStoreDB(configuration)
            .AddProjections(assembliesToScan);
    }
}