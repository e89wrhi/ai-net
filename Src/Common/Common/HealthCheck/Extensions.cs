
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;
using AI.Common.EFCore;
using AI.Common.EventStoreDB;
using AI.Common.MassTransit;
using AI.Common.Web;

namespace AI.Common.HealthCheck;

/// <summary>
/// Infrastructure extensions for setting up Health Checks.
/// It configures checks for RabbitMQ, Postgres, and EventStoreDB,
/// and also provides a UI to visualize the system's health status.
/// </summary>
public static class Extensions
{
    private const string HealthEndpointPath = "/health";
    private const string AlivenessEndpointPath = "/alive";

    public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services)
    {
        var healthOptions = services.GetOptions<HealthOptions>(nameof(HealthOptions));

        if (healthOptions.Enabled)
        {
            var appOptions = services.GetOptions<AppOptions>(nameof(AppOptions));
            var postgresOptions = services.GetOptions<PostgresOptions>(nameof(PostgresOptions));
            var rabbitMqOptions = services.GetOptions<RabbitMqOptions>(nameof(RabbitMqOptions));
            var eventStoreOptions = services.GetOptions<EventStoreOptions>(nameof(EventStoreOptions));
            
            var healthChecksBuilder = services.AddHealthChecks()
                // Add a default liveness check to ensure app is responsive
                .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"])
                .AddRabbitMQ(
                    serviceProvider =>
                    {
                        var factory = new ConnectionFactory
                        {
                            Uri = new Uri($"amqp://{rabbitMqOptions.UserName}:{rabbitMqOptions.Password}@{rabbitMqOptions.HostName}"),
                        };
                        return factory.CreateConnectionAsync();
                    });

            if (!string.IsNullOrEmpty(postgresOptions.ConnectionString))
                healthChecksBuilder.AddNpgSql(postgresOptions.ConnectionString);

            if (!string.IsNullOrEmpty(eventStoreOptions.ConnectionString))
                healthChecksBuilder.AddEventStore(eventStoreOptions.ConnectionString);

            services.AddHealthChecksUI(setup =>
                                       {
                                           setup.SetEvaluationTimeInSeconds(60); // time in seconds between check
                                           setup.AddHealthCheckEndpoint($"Self Check - {appOptions.Name}", HealthEndpointPath);
                                       }).AddInMemoryStorage();
        }

        services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);
        return services;
    }

    public static WebApplication UseCustomHealthCheck(this WebApplication app)
    {
        var healthOptions = app.Configuration.GetOptions<HealthOptions>(nameof(HealthOptions));

        if (app.Environment.IsDevelopment())
        {
            app.MapHealthChecks(HealthEndpointPath);
            app.MapHealthChecks(AlivenessEndpointPath, new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live"),
            });
        }

        if (healthOptions.Enabled)
            app.MapHealthChecksUI(options => options.UIPath = "/health-ui");

        return app;
    }
}